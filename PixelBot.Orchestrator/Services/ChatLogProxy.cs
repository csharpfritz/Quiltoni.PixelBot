using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PixelBot.Orchestrator.Services
{
	public class ChatLogProxy
	{
		// IHubContext<LoggerHub, IChatLogger> chatContext) {  //
		public ChatLogProxy(IServiceProvider provider) {

			this.Provider = provider;
			//ChatLogger = chatContext;

		}

		private IServiceProvider Provider;

		private IHubContext<LoggerHub, IChatLogger> ChatLogger {
			get {

				using (var scope = Provider.CreateScope()) {
					return scope.ServiceProvider.GetRequiredService<IHubContext<LoggerHub, IChatLogger>>();
				}
			}
			//get;
		}

		public async Task LogMessage(LogLevel level, string message) {

			await ChatLogger.Clients.All.LogMessage(level.ToString(), message);

		}


	}
}
