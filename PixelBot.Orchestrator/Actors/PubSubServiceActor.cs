using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace PixelBot.Orchestrator.Actors
{

	public class PubSubServiceActor : ReceiveActor
	{

		public enum PubSubEvent {
			Subscriptions
		}

		private readonly TwitchPubSub _Client;

		private readonly List<(string channelId, string channelName, PubSubEvent evt)> _SubscribedChannels = new List<(string channelId, string channelName, PubSubEvent evt)>();

		public PubSubServiceActor()
		{

			_Client = new TwitchPubSub();
			_Client.OnChannelSubscription += _Client_OnChannelSubscription;

			Receive<TrackSubscribers>(ListenToSubscriptionsForChannel);

		}

		public override void AroundPreRestart(Exception cause, object message)
		{

			_Client.Connect();

			base.AroundPreRestart(cause, message);
		}

		public override void AroundPostStop()
		{

			_Client.Disconnect();

			base.AroundPostStop();
		}


		private void ListenToSubscriptionsForChannel(TrackSubscribers msg)
		{

			_SubscribedChannels.Add((msg.ChannelId, msg.ChannelName, PubSubEvent.Subscriptions));

			_Client.ListenToSubscriptions(msg.ChannelId);

		}

		private void _Client_OnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
		{

			// Inspect the notified subscription and trigger a SignalR hub with information about the subscription
			ChannelManagerActor.Instance.Tell(new ReportNewSubscriberForChannel(e.Subscription.ChannelName, e.Subscription.ChannelId, e.Subscription.RecipientName, e.Subscription.RecipientId, e.Subscription.SubMessage.Message, (short)e.Subscription.Months));
			


		}


	}

}
