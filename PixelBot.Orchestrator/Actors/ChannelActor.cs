using System;
using System.Collections.Generic;
using Akka.Actor;
using PixelBot.Orchestrator.Actors.ChannelEvents;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// An actor that manages interactions on a single channel
	/// </summary>
	public class ChannelActor : ReceiveActor
	{

		private TwitchClient _Client;
		
		private IActorRef ChatCommand;
		private IActorRef GiftSub;
		private IActorRef NewMessage;
		private IActorRef NewSub;
		private IActorRef Raid;
		private IActorRef ReSub;

		public ChannelActor(ChannelConfiguration config) {

			this.Config = config;

			Receive<MSG.WhisperMessage>(msg => WhisperMessage(msg));
			Receive<MSG.BroadcastMessage>(msg => BroadcastMessage(msg));

		}

		private void BroadcastMessage(MSG.BroadcastMessage msg) {

			_Client.SendMessage(Config.ChannelName, msg.Message);

		}

		private void WhisperMessage(MSG.WhisperMessage msg) {

			_Client.SendWhisper(msg.UserToWhisper, msg.Message);

		}

		public ChannelConfiguration Config { get; }

		public BotConfiguration BotConfig { get; } = Startup.BotConfiguration;

		public override void AroundPreStart() {

			StartTwitchConnection();
			base.AroundPreStart();

		}

		private void StartTwitchConnection() {

			var creds = new ConnectionCredentials(BotConfig.LoginName, BotConfig.Password);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += (sender, args) => _Client.JoinChannel(Config.ChannelName);

			// TODO: Handle unwanted disconnect
			// Cheer 142 cpayette 25/4/19 

			StartEventHandlerActors();

			_Client.OnNewSubscriber += (o, args) => NewSub.Forward(args);
			_Client.OnReSubscriber += (o, args) => ReSub.Forward(args);
			_Client.OnGiftedSubscription += (o, args) => GiftSub.Forward(args);
			_Client.OnRaidNotification += (o, args) => Raid.Forward(args);
			_Client.OnChatCommandReceived += (o, args) => ChatCommand.Forward(args);
			_Client.OnMessageReceived += (o, args) => NewMessage.Forward(args);

			_Client.Connect();

		}

		private void StartEventHandlerActors() {

			this.ChatCommand = CreateActor<ChatCommandActor>();
			this.GiftSub = CreateActor<GiftSubscriberActor>();
			this.NewMessage = CreateActor<NewMessageActor>();
			this.NewSub = CreateActor<NewSubscriberActor>();
			this.Raid = CreateActor<RaidActor>();
			this.ReSub = CreateActor<ReSubscriberActor>();

			IActorRef CreateActor<T>() where T : ReceiveActor
			{

				var props = Props.Create<T>(Config);
				return Context.ActorOf(props, $"event_{typeof(T).Name}");

			}

		}

		public override void AroundPostStop() {
			_Client.Disconnect();
			base.AroundPostStop();
		}

	}

}
