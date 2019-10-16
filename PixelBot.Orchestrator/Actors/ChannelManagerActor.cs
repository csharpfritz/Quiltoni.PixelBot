using Akka.Actor;
using Akka.Event;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using System.Collections.Generic;
using System.Linq;
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

		public static IActorRef Create(ActorSystem system)
		{

			var props = Props.Create<ChannelManagerActor>();
			Instance = system.ActorOf(props, Name);
			return Instance;

		}

		public ChannelManagerActor()
		{
			Logger = Context.GetLogger();

			_ChatLogger = ChatLoggerActor.Create();

			CreateFollowerActor();
			_ChannelConfigurationActor = Context.ActorOf(
				Props.Create<ChannelConfigurationActor>(), nameof(ChannelConfigurationActor));

			Receive<JoinChannel>(this.GetChannelActor);
			ReceiveAsync<LeaveChannel>(this.LeaveChannel);

			Receive<ReportCurrentChannels>(_ =>
			{
				Sender.Tell(_ChannelActors.Select(kv => kv.Key).ToArray());
			});
			Receive<OnNewFollowersDetectedArgs>(args =>
			{

				_ChannelActors[args.Channel].Tell(args);

			});
			Receive<NotifyChannelOfConfigurationUpdate>(UpdateChannelWithConfiguration);

			Receive<IsChannelConnected>(c => Sender.Tell(new IsChannelConnectedResponse(_ChannelActors.ContainsKey(c.ChannelName))));

			Receive<GetConfigurationForChannel>(msg =>
			{

				var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(msg);
				Sender.Tell(config);

			});

			//Receive<GetFeatureForChannel>(async f => {
			//	var theChannelActor = _ChannelActors[f.Channel];
			//	Sender.Tell(await theChannelActor.Ask(new GetFeatureFromChannel(f.FeatureType)));
			//});

		}

		private void UpdateChannelWithConfiguration(NotifyChannelOfConfigurationUpdate msg)
		{

			if (!_ChannelActors.ContainsKey(msg.ChannelName))
				return;

			_ChannelActors[msg.ChannelName].Tell(msg);

		}

		private void CreateFollowerActor()
		{
			_FollowerActor = Context.ActorOf(Props.Create<FollowerServiceActor>());
		}

		public ILoggingAdapter Logger { get; }

		private bool GetChannelActor(JoinChannel msg)
		{

			if (string.IsNullOrEmpty(msg.ChannelName))
				return false;

			if (_ChannelActors.ContainsKey(msg.ChannelName))
			{
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return false;
			}

			var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(msg.ChannelName)).GetAwaiter().GetResult();

			var child = Context.ActorOf(ChannelActor.Props(config), $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

			// Track followers for that channel?
			_FollowerActor.Tell(new TrackNewFollowers(msg.ChannelName, config.ChannelId));

			return true;

		}

		private async Task LeaveChannel(LeaveChannel msg)
		{

			if (!_ChannelActors.ContainsKey(msg.ChannelName))
				return;

			var actor = _ChannelActors[msg.ChannelName];
			await actor.GracefulStop(TimeSpan.FromSeconds(10));

			Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' has been stopped.");
			_ChannelActors.Remove(msg.ChannelName);

			_FollowerActor.Tell(new StopTrackingFollowers(msg.ChannelName, ""));

		}



		public ChannelActor this[string channelName]
		{
			get { return _ChannelActors[channelName] as ChannelActor; }
		}

	}
}
