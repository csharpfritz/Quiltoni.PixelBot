using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Api.Services.Events.FollowerService;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// Manage a collection of Bot actors that interact with the individual channels
	/// </summary>
	public class ChannelManagerActor : ReceiveActor
	{

		private static readonly Dictionary<string, IActorRef> _ChannelActors = new Dictionary<string, IActorRef>();

		private readonly IActorRef _ChatLogger;
		private readonly IActorRef _ChannelConfigurationActor;
		private IActorRef _FollowerActor;
		public const string Name = "channelmanager";

		public static IActorRef Instance { get; private set; }

		public static IActorRef Create(
			ActorSystem system, 
			IChannelConfigurationContext dataContext, 
			IHubContext<LoggerHub, IChatLogger> chatLogger) {

			var props = Props.Create<ChannelManagerActor>(dataContext, chatLogger);
			Instance = system.ActorOf(props, Name);
			return Instance;

		}

		public ChannelManagerActor(IChannelConfigurationContext dataContext, IHubContext<LoggerHub, IChatLogger> chatLogger) {

			Logger = Context.GetLogger();

			_ChatLogger = ChatLoggerActor.Create(chatLogger);

			CreateFollowerActor();
			_ChannelConfigurationActor = Context.ActorOf(Props.Create<ChannelConfigurationActor>(dataContext));

			Receive<JoinChannel>(this.GetChannelActor);
			Receive<ReportCurrentChannels>(_ => {
				 Sender.Tell(_ChannelActors.Select(kv => kv.Key).ToArray());
			});
			Receive<OnNewFollowersDetectedArgs>(args => {

				_ChannelActors[args.Channel].Tell(args);

			});
			Receive<NotifyChannelOfConfigurationUpdate>(UpdateChannelWithConfiguration);

			Receive<GetConfigurationForChannel>(msg =>
			{

				var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(msg);
				Sender.Tell(config);

			});

			//Receive<GetFeatureForChannel>(async f => {
			//	var theChannelActor = _ChannelActors[f.Channel];
			//	Sender.Tell(await theChannelActor.Ask(new GetFeatureFromChannel(f.FeatureType)));
			//});
			this.DataContext = dataContext;

		}

		private void UpdateChannelWithConfiguration(NotifyChannelOfConfigurationUpdate msg)
		{

			_ChannelActors[msg.ChannelName].Tell(msg);

		}

		private void CreateFollowerActor() {

			_FollowerActor = Context.ActorOf<FollowerServiceActor>();

		}

		public IChannelConfigurationContext DataContext { get; }

		public ILoggingAdapter Logger { get; }

		private bool GetChannelActor(JoinChannel msg) {

			if (string.IsNullOrEmpty(msg.ChannelName)) return false;

			if (_ChannelActors.ContainsKey(msg.ChannelName)) {
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return false;
			}

			var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(msg.ChannelName)).GetAwaiter().GetResult();

			var child = Context.ActorOf(ChannelActor.Props(config), $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

			return true;

		}

		public ChannelActor this[string channelName] {
			get { return _ChannelActors[channelName] as ChannelActor; }
		}

	}
}
