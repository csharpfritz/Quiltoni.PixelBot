using System;
using System.Collections.Generic;
using Akka.Actor;
using PixelBot.Orchestrator.Actors.ChannelEvents;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Data;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using System.Reflection;
using MSG = Quiltoni.PixelBot.Core.Messages;
using Quiltoni.PixelBot.Core.Messages.Currency;
using System.Linq;

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
			Receive<MSG.Currency.AddCurrencyMessage>(msg => AddCurrency(msg));
			Receive<MSG.Currency.MyCurrencyMessage>(msg => ReportCurrency(msg));

			Self = Context.Self;

		}

		private void AddCurrency(AddCurrencyMessage msg) {
			if (msg.UserName.StartsWith("#")) {
				CurrencyRepository.AddForChatters(msg.UserName.Substring(1), msg.Amount, msg.ActingUser);
			} else {
				CurrencyRepository.AddForUser(msg.UserName, msg.Amount, msg.ActingUser);
			}
		}

		private void ReportCurrency(MyCurrencyMessage msg) {

			var count = CurrencyRepository.FindForUser(msg.UserName);
			BroadcastMessage(new MSG.BroadcastMessage($"{msg.UserName} has {count} {Config.Currency.Name}"));

		}

		private void BroadcastMessage(MSG.BroadcastMessage msg) {

			_Client.SendMessage(Config.ChannelName, msg.Message);

		}

		private void WhisperMessage(MSG.WhisperMessage msg) {

			_Client.SendWhisper(msg.UserToWhisper, msg.Message);

		}

		public ChannelConfiguration Config { get; }

		public BotConfiguration BotConfig { get; } = Startup.BotConfiguration;

		public ICurrencyRepository CurrencyRepository { get; set; }

		public override void AroundPreStart() {

			StartTwitchConnection();
			base.AroundPreStart();

		}

		public IActorRef Self { get; private set; }

		private void StartTwitchConnection() {

			var creds = new ConnectionCredentials(BotConfig.LoginName, BotConfig.Password);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += (sender, args) => _Client.JoinChannel(Config.ChannelName);

			ConfigureCurrencyRepository();

			// TODO: Handle unwanted disconnect
			// Cheer 142 cpayette 25/4/19 

			StartEventHandlerActors();

			_Client.OnNewSubscriber += (o, args) => NewSub.Tell(args, this.Self);
			_Client.OnReSubscriber += (o, args) => ReSub.Tell(args, this.Self);
			_Client.OnGiftedSubscription += (o, args) => GiftSub.Tell(args, this.Self);
			_Client.OnRaidNotification += (o, args) => Raid.Tell(args, this.Self);
			_Client.OnChatCommandReceived += (o, args) => ChatCommand.Tell(args, this.Self);
			_Client.OnMessageReceived += (o, args) => NewMessage.Tell(args, this.Self);

			_Client.Connect();

		}

		private void StartEventHandlerActors() {

			this.ChatCommand = CreateActor<ChatCommandActor>();
			this.GiftSub = CreateActor<GiftSubscriberActor>(CurrencyRepository);
			this.NewMessage = CreateActor<NewMessageActor>();
			this.NewSub = CreateActor<NewSubscriberActor>(CurrencyRepository);
			this.Raid = CreateActor<RaidActor>(CurrencyRepository);
			this.ReSub = CreateActor<ReSubscriberActor>(CurrencyRepository);

			IActorRef CreateActor<T>(params object[] args) where T : ReceiveActor {

				var realArgs = new object[] { Config };
				realArgs = realArgs.Concat(args).ToArray();
				var props = Akka.Actor.Props.Create<T>(realArgs);
				return Context.ActorOf(props, $"event_{typeof(T).Name}");

			}

		}

		public override void AroundPostStop() {
			_Client.Disconnect();
			base.AroundPostStop();
		}

		public static Props Props(ChannelConfiguration config) {
				return Akka.Actor.Props.Create<ChannelActor>(config);
		}

		private void ConfigureCurrencyRepository() {

			if (!Config.Currency.Enabled) return;

			if (Config.Currency.Google != null) {

				var sheetType = "PixelBot.Google, " + Config.Currency.Google.RepositoryType.ToString();

				// TODO: Need to grab the Akka logger factory and pass in as the third argument
				CurrencyRepository = Activator.CreateInstance(Type.GetType(sheetType), Config, null) as ICurrencyRepository;

			}

		}

	}

}
