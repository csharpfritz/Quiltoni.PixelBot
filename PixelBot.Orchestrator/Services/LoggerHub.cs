using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace PixelBot.Orchestrator.Services
{

	//[Authorize(Policy = nameof(Policy.GlobalAdmin))]
	public class LoggerHub : Hub
	{

		//[Authorize(nameof(Policy.GlobalAdmin))]
		//public async Task LogMessage(LogLevel level, string message) {

		//	await Clients.All.LogMessage(level, message);

		//}

	}

	public interface IChatLogger
	{

		Task LogMessage(LogLevel level, string message);

	}

}
