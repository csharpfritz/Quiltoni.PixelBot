using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private readonly IWebHostEnvironment env;

		public TestController(IHubContext<UserActivityHub, IUserActivityClient> context, IWebHostEnvironment env)
		{

			this.HubContext = context;
			this.env = env;
		}

		public IHubContext<UserActivityHub, IUserActivityClient> HubContext { get; }

		public IHubContext<LoggerHub, IChatLogger> LoggerContext { get; }

		public async Task<IActionResult> Get(string followerName)
		{

			if (env.IsDevelopment())
			{
				await HubContext.Clients.All.NewFollower(followerName);
			}
			return Ok();

		}

	}
}