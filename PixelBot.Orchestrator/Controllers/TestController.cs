using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{

		public TestController(IActorRef channelMgr, IHubContext<LoggerHub> loggerContext) {

			this.ChannelManager = channelMgr;
			this.LoggerContext = loggerContext;

		}

		public IActorRef ChannelManager { get; }
		public IHubContext<LoggerHub> LoggerContext { get; }

		public IActionResult Get(string channelName) {

			ChannelManager.Tell(new JoinChannel(channelName));
			LoggerContext.Clients.All.SendAsync("LogMessage", LogLevel.Information, "Test message").GetAwaiter().GetResult();
			return Ok();

		}

	}
}