using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{
	public class SubscriptionManagerActor : ReceiveActor
	{
		private readonly IServiceProvider _Services;

		public SubscriptionManagerActor(IServiceProvider services)
		{

			_Services = services;

			ReceiveAsync<ReportNewSubscriberForChannel>(OnNewSubscriberForChannel);

		}

		private async Task OnNewSubscriberForChannel(ReportNewSubscriberForChannel message)
		{

			// Dedupe -- did we already receive this message?

			await NotifySignalR(message);

		}

		private async Task NotifySignalR(ReportNewSubscriberForChannel message) {

			using (var scope = _Services.CreateScope()) {

				var client = scope.ServiceProvider.GetRequiredService<IHubContext<UserActivityHub, IUserActivityClient>>();
				await client.Clients.Group(message.ChannelName).NewSubscriber(message.UserName, message.NumberOfMonths, 0, message.Message);

			}

		}

	}

}
