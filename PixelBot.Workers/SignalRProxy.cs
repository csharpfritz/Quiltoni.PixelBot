using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace PixelBot.Workers
{

	public class SignalRProxy : IOrchestratorProxy
	{

		public SignalRProxy(IConfiguration config) {

			this.OrchestratorUrl = new Uri(config["OrchestratorUrl"]);

		}

		/// <summary>
		/// The location of the orchestrator we are working for
		/// </summary>
		public Uri OrchestratorUrl { get; }

		public Action<string> ConnectToRoom { get; set; } = null;

		private HubConnection Connection;

		public async Task ConnectAsync() {

			// Cheer 200 roberttables 16/4/19 
			// Cheer 200 cpayette 16/4/19 

			this.Connection = new HubConnectionBuilder()
				.WithUrl(OrchestratorUrl)
				.Build();

			// TODO: Configure the various messages returned from the orchestrator
			Connection.On<string>("ConnectToRoom", ConnectToRoom);


			Connection.Closed += async (error) => {

				// No error means the connection was closed on purpose
				if (error == null) return;

				if (TooManyErrors(error)) return;

				await Task.Delay(2000);
				await Connection.StartAsync();

			};

			await Connection.StartAsync();

		}

		private bool TooManyErrors(Exception error) {

			// TODO: Add logging and diagnostics

			return false;
		}

		public async Task DisconnectAsync() {
			await Connection?.DisposeAsync();
		}

	}

}
