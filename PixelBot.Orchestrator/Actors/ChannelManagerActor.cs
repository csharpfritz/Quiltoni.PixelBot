using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
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
			IServiceProvider serviceProvider)
		{

			var props = Props.Create<ChannelManagerActor>(serviceProvider);
			Instance = system.ActorOf(props, Name);
			return Instance;

		}

		public ChannelManagerActor(IServiceProvider serviceProvider)
		{

			this.ServiceProvider = serviceProvider;

			Logger = Context.GetLogger();
			_HttpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

			_ChatLogger = ChatLoggerActor.Create(serviceProvider.GetService<IHubContext<LoggerHub, IChatLogger>>());

			CreateFollowerActor();
			_ChannelConfigurationActor = Context.ActorOf(
				Props.Create<ChannelConfigurationActor>(
					serviceProvider.GetService<IChannelConfigurationContext>(),
					_HttpClientFactory),
					nameof(ChannelConfigurationActor));

			ConfigureMessageReceiveStatements();

			//Receive<GetFeatureForChannel>(async f => {
			//	var theChannelActor = _ChannelActors[f.Channel];
			//	Sender.Tell(await theChannelActor.Ask(new GetFeatureFromChannel(f.FeatureType)));
			//});

		}

		private async Task RejoinChannels(RejoinChannels msg)
		{

			var rejoinList = await _ChannelConfigurationActor.Ask<ChannelsToReconnect>(new GetChannelsToReconnect());
			Parallel.ForEach(rejoinList.Channels, channel => GetChannelActor(new JoinChannel(channel)));

		}

		private void ConfigureMessageReceiveStatements()
		{
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

			ReceiveAsync<RejoinChannels>(RejoinChannels);

		}

		private void UpdateChannelWithConfiguration(NotifyChannelOfConfigurationUpdate msg)
		{

			if (!_ChannelActors.ContainsKey(msg.ChannelName)) return;

			_ChannelActors[msg.ChannelName].Tell(msg);

		}

		private void CreateFollowerActor()
		{


			_FollowerActor = Context.ActorOf(Props.Create<FollowerServiceActor>(new object[] {
				_HttpClientFactory,
				ServiceProvider.GetService<IConfiguration>(),
				ServiceProvider.GetService<IWebHostEnvironment>()
			}));

		}

		public IServiceProvider ServiceProvider { get; }
		public ILoggingAdapter Logger { get; }

		private readonly IHttpClientFactory _HttpClientFactory;

		private bool GetChannelActor(JoinChannel msg)
		{

			if (string.IsNullOrEmpty(msg.ChannelName)) return false;

			if (_ChannelActors.ContainsKey(msg.ChannelName))
			{
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return false;
			}

			var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(msg.ChannelName)).GetAwaiter().GetResult();
			if (!config.ConnectedToChannel)
			{
				config.ConnectedToChannel = true;
				_ChannelConfigurationActor.Tell(new SaveConfigurationForChannel(msg.ChannelName, config));
			}

			var child = Context.ActorOf(ChannelActor.Props(config), $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

			// Track followers for that channel?
			_FollowerActor.Tell(new TrackNewFollowers(msg.ChannelName, config.ChannelId));

			return true;

		}

		private async Task LeaveChannel(LeaveChannel msg)
		{

			if (!_ChannelActors.ContainsKey(msg.ChannelName)) return;

			var actor = _ChannelActors[msg.ChannelName];
			await actor.GracefulStop(TimeSpan.FromSeconds(10));

			var config = _ChannelConfigurationActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(msg.ChannelName)).GetAwaiter().GetResult();
			if (config.ConnectedToChannel)
			{
				config.ConnectedToChannel = false;
				_ChannelConfigurationActor.Tell(new SaveConfigurationForChannel(msg.ChannelName, config));
			}


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