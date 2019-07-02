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
		// IHubContext<LoggerHub, IChatLogger> loggerContext
		public TestController(IActorRef channelMgr) {

			this.ChannelManager = channelMgr;

		}

		public IActorRef ChannelManager { get; }

		public IHubContext<LoggerHub, IChatLogger> LoggerContext { get; }

		public async Task<IActionResult> Get(string channelName) {

			ChannelManager.Tell(new JoinChannel(channelName));
			return Ok();

		}

	}
}