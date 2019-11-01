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
using Microsoft.Extensions.DependencyInjection;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Extensibility;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;
using Akka.Event;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// An actor that manages interactions on a single channel
	/// </summary>
	public class ChannelActor : ReceiveActor
	{

		private TwitchClient _Client;
		private PluginBootstrapper _Bootstrapper;

		private IActorRef _ChatCommand;
		private IActorRef _GiftSub;
		private IActorRef _NewMessage;
		private IActorRef _NewSub;
		private IActorRef _Raid;
		private IActorRef _ReSub;
		private IActorRef _NewFollower;
		private TwitchAPI _API;
		private FollowerService _FollowerService;

		private IActorRef[] EventActors { get { return new[] { _ChatCommand, _GiftSub, _NewMessage, _NewSub, _Raid, _ReSub }; } }

		public ChannelActor(ChannelConfiguration config)
		{

			this.Config = config;
			this._Bootstrapper = new PluginBootstrapper(config);

			Receive<MSG.WhisperMessage>(msg => WhisperMessage(msg));
			Receive<MSG.BroadcastMessage>(msg => BroadcastMessage(msg));
			Receive<MSG.Currency.AddCurrencyMessage>(msg => AddCurrency(msg));
			Receive<MSG.Currency.MyCurrencyMessage>(msg => ReportCurrency(msg));
			Receive<OnNewFollowersDetectedArgs>(args => _NewFollower.Tell(args, this.Self));
			Receive<MSG.NotifyChannelOfConfigurationUpdate>(msg => this.Config = config);
			//Receive<MSG.GetFeatureFromChannel>(msg => Sender.Tell(GetFeature(msg.FeatureType)));

			Self = Context.Self;

		}

		private void AddCurrency(AddCurrencyMessage msg)
		{
			if (msg.UserName.StartsWith("#"))
			{
				CurrencyRepository.AddForChatters(msg.UserName.Substring(1), msg.Amount, msg.ActingUser);
			}
			else
			{
				CurrencyRepository.AddForUser(msg.UserName, msg.Amount, msg.ActingUser);
			}
		}

		private void ReportCurrency(MyCurrencyMessage msg)
		{

			var count = CurrencyRepository.FindForUser(msg.UserName);
			BroadcastMessage(new MSG.BroadcastMessage($"{msg.UserName} has {count} {Config.Currency.Name}"));

		}

		private void BroadcastMessage(MSG.BroadcastMessage msg)
		{

			_Client.SendMessage(Config.ChannelName, msg.Message);

		}

		private void WhisperMessage(MSG.WhisperMessage msg)
		{

			_Client.SendWhisper(msg.UserToWhisper, msg.Message);

		}

		public ChannelConfiguration Config { get; private set; }

		public BotConfiguration BotConfig { get; } = Startup.BotConfiguration;

		public ICurrencyRepository CurrencyRepository { get; set; }

		public override void AroundPreStart()
		{

			StartTwitchConnection();
			base.AroundPreStart();

		}

		public IActorRef Self { get; private set; }

		private void StartTwitchConnection()
		{

			var logger = Context.GetLogger();
			logger.Log(LogLevel.DebugLevel, $"Connecting to channel {Config.ChannelName} with bot username {BotConfig.LoginName}");

			var creds = new ConnectionCredentials(BotConfig.LoginName, BotConfig.Password);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += (sender, args) => _Client.JoinChannel(Config.ChannelName);

			ConfigureCurrencyRepository();

			// TODO: Handle unwanted disconnect
			// Cheer 142 cpayette 25/4/19 

			StartEventHandlerActors();

			_Client.OnNewSubscriber += (o, args) => _NewSub.Tell(args, this.Self);
			_Client.OnReSubscriber += (o, args) => _ReSub.Tell(args, this.Self);
			_Client.OnGiftedSubscription += (o, args) => _GiftSub.Tell(args, this.Self);
			_Client.OnRaidNotification += (o, args) => _Raid.Tell(args, this.Self);
			_Client.OnChatCommandReceived += (o, args) => _ChatCommand.Tell(args, this.Self);
			_Client.OnMessageReceived += (o, args) => _NewMessage.Tell(args, this.Self);

			_Client.Connect();

		}

		private void StartEventHandlerActors()
		{

			// TODO: Inject features appropriate for each StreamEvent

			this._ChatCommand = CreateActor<ChatCommandActor>(StreamEvent.OnCommand);
			this._GiftSub = CreateActor<GiftSubscriberActor>(StreamEvent.OnGiftSubscribe, CurrencyRepository);
			this._NewMessage = CreateActor<NewMessageActor>(StreamEvent.OnMessage);
			this._NewSub = CreateActor<NewSubscriberActor>(StreamEvent.OnSubscribe, CurrencyRepository);
			this._Raid = CreateActor<RaidActor>(StreamEvent.OnRaid, CurrencyRepository);
			this._ReSub = CreateActor<ReSubscriberActor>(StreamEvent.OnResubscribe, CurrencyRepository);
			this._NewFollower = CreateActor<NewFollowerActor>(StreamEvent.OnFollow);

			IActorRef CreateActor<T>(StreamEvent evt, params object[] args) where T : ReceiveActor
			{

				var features = _Bootstrapper.GetFeaturesForStreamEvent(evt);
				var realArgs = new object[] { Config, features };
				realArgs = realArgs.Concat(args).ToArray();
				var props = Akka.Actor.Props.Create<T>(realArgs);

				return Context.ActorOf(props, $"event_{typeof(T).Name}");

			}

		}

		public override void AroundPostStop()
		{
			_Client.Disconnect();
			base.AroundPostStop();
		}

		public static Props Props(ChannelConfiguration config)
		{
			return Akka.Actor.Props.Create<ChannelActor>(config);
		}

		private void ConfigureCurrencyRepository()
		{

			if (!Config.Currency.Enabled) return;

			if (Config.Currency.Google != null)
			{

				var sheetType = "PixelBot.Google, " + Config.Currency.Google.RepositoryType.ToString();

				// TODO: Need to grab the Akka logger factory and pass in as the third argument
				CurrencyRepository = Activator.CreateInstance(Type.GetType(sheetType), Config, null) as ICurrencyRepository;

			}

		}

		public IFeature GetFeature(Type featureType)
		{

			foreach (var a in EventActors)
			{

				if (a.GetType().GetProperty("Features") != null)
				{

					var featureProperty = a.GetType().GetProperty("Features");
					var features = featureProperty.GetValue(a) as IFeature[];
					return features.FirstOrDefault(f => f.GetType() == featureType);

				}

			}

			return null;

		}

	}

}