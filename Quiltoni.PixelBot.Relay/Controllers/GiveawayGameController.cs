using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Quiltoni.PixelBot.Core;

namespace Quiltoni.PixelBot.Relay.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class GiveawayGameController : ControllerBase
	{
		private IHubContext<NotificationHub, IOrderNotificationClient> _HubContext;

		public GiveawayGameController(IHubContext<NotificationHub, IOrderNotificationClient> hubContext) {
			_HubContext = hubContext;
		}

		[HttpPost()]
		public async Task<IActionResult> Post([FromBody]string[] entrants) {

			await _HubContext.Clients.All.RunRaffle(entrants);
			return Ok();

		}


	}

}
