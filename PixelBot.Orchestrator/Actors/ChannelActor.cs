using System;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// An actor that manages interactions on a single channel
	/// </summary>
	public class ChannelActor : ReceiveActor, IDisposable
	{

		private TwitchClient _Client;

		public ChannelActor(ChannelConfiguration config) {

			this.Config = config;

		}

		public ChannelConfiguration Config { get; }

		public BotConfiguration BotConfig { get; }

		public override void AroundPreStart() {

			StartTwitchConnection();
			base.AroundPreStart();

		}

		private void StartTwitchConnection() {

			var creds = new ConnectionCredentials(BotConfig.LoginName, BotConfig.Password);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += (sender, args) => _Client.JoinChannel(Config.ChannelName);

			// Delegate these to another actor class

			// IActorRef.Tell() to delegate these messages to another class

			_Client.OnNewSubscriber += _Client_OnNewSubscriber;
			_Client.OnReSubscriber += _Client_OnReSubscriber;
			_Client.OnGiftedSubscription += _Client_OnGiftedSubscription;
			_Client.OnRaidNotification += _Client_OnRaidNotification;
			_Client.OnChatCommandReceived += _Client_OnChatCommandReceived;
			_Client.OnMessageReceived += _Client_OnMessageReceived;

			_Client.Connect();

		}

		private void _Client_OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			throw new NotImplementedException();
		}

		private void _Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e) {
			throw new NotImplementedException();
		}

		private void _Client_OnRaidNotification(object sender, OnRaidNotificationArgs e) {
			throw new NotImplementedException();
		}

		private void _Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e) {
			throw new NotImplementedException();
		}

		private void _Client_OnReSubscriber(object sender, OnReSubscriberArgs e) {
			throw new NotImplementedException();
		}

		private void _Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e) {
			throw new NotImplementedException();
		}

		public override void AroundPostStop() {
			base.AroundPostStop();
		}


		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					


				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {

			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);

		}
		#endregion

	}

}
