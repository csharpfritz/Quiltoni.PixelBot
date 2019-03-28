using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Relay.Models;

namespace Quiltoni.PixelBot.Relay.Controllers
{
	[Route("api/[controller]")]
	//[ApiController()]
	public class ShopifyController : ControllerBase
	{

		public ShopifyController(IOptions<StoreConfig> config,
				ILogger<ShopifyController> logger,
				IHubContext<NotificationHub, IOrderNotificationClient> hubContext)
		{
			this.HubContext = hubContext;
			this.Logger = logger;
			this.StoreConfigs = config.Value.Shopify.ToArray();
		}

		public IHubContext<NotificationHub, IOrderNotificationClient> HubContext { get; }
		public ILogger<ShopifyController> Logger { get; }
		public Models.ShopifyStore[] StoreConfigs { get; }

		// POST api/values
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Models.Order newOrder)
		{

			if (!ModelState.IsValid) {
				Logger.LogError($"ModelBinding failed: {ControllerContext.ModelState.Values.First(v => v.Errors.Any()).Errors.First().ErrorMessage}");

				var body = ControllerContext.HttpContext.Request.Body;
				body.Position = 0;
				var sr = new StreamReader(body);

				Logger.LogError(sr.ReadToEnd());
				return new BadRequestObjectResult(ControllerContext.ModelState);
			}

			if (!await VerifyHeader(Request))
			{
				Logger.LogError("Header did not pass verification");
				return BadRequest();
			}

			Logger.LogDebug($"Sending notification of Order ({newOrder.id}) to {NotificationHub.ConnectedCount} client(s)");
			await HubContext.Clients.All.OrderReceived(newOrder.id.ToString());

			return Ok();

		}

		private async Task<bool> VerifyHeader(HttpRequest request)
		{

			var topicHeader = request.Headers["X-Shopify-Topic"].FirstOrDefault();
			if (!topicHeader.StartsWith("orders/")) {
				Logger.LogError($"Topic was not an Order: {topicHeader}");
				return false;
			}

			var domainHeader = request.Headers["X-Shopify-Shop-Domain"].FirstOrDefault();
			if (!StoreConfigs.Any(s => (domainHeader == s.Name))) {
				Logger.LogError($"Message was not sent from a domain we manage: {domainHeader}");
				return false;
			}

			var theStore = StoreConfigs.First(s => s.Name == domainHeader);

			var hmacHeader = request.Headers["X-Shopify-Hmac-SHA256"].FirstOrDefault();
			if (string.IsNullOrEmpty(hmacHeader))
			{
				Logger.LogError("Shopify-HMAC header not found");
				return false;
			}

			// TODO: HMAC verify
			var ourHashCalculation = string.Empty;
			using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
			{
				Request.Body.Position = 0;
				var bodyContent = await reader.ReadToEndAsync();
				ourHashCalculation = CreateHmacHash(bodyContent, theStore.Key);
			}
			return ourHashCalculation == hmacHeader;

		}

		private static string CreateHmacHash(string data, string key)
		{

			var keybytes = UTF8Encoding.UTF8.GetBytes(key);
			var dataBytes = UTF8Encoding.UTF8.GetBytes(data);

			var hmac = new HMACSHA256(keybytes);
			var hmacBytes = hmac.ComputeHash(dataBytes);

			return Convert.ToBase64String(hmacBytes);

		}
	}
}
