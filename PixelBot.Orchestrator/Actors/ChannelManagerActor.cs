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
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// Manage a collection of Bot actors that interact with the individual channels
	/// </summary>
	public class ChannelManagerActor : ReceiveActor
	{

		private static readonly Dictionary<string, IActorRef> _ChannelActors = new Dictionary<string, IActorRef>();

		private readonly IActorRef _ChatLogger;
		private readonly IActorRef _PluginBootstrapper;
		public const string Name = "channelmanager";

		public static IActorRef Create(
			ActorSystem system, 
			IChannelConfigurationContext dataContext, 
			IHubContext<LoggerHub, IChatLogger> chatLogger,
			IServiceProvider serviceProvider) {

			var props = Props.Create<ChannelManagerActor>(dataContext, chatLogger, serviceProvider);
			return system.ActorOf(props, Name);

		}

		public ChannelManagerActor(IChannelConfigurationContext dataContext, IHubContext<LoggerHub, IChatLogger> chatLogger, IEnumerable<Type> featureTypes, IServiceProvider serviceProvider) {

			Logger = Context.GetLogger();

			_ChatLogger = ChatLoggerActor.Create(chatLogger);
			_PluginBootstrapper = PluginBootstrapper.Create(serviceProvider);

			Receive<JoinChannel>(this.GetChannelActor);
			Receive<ReportCurrentChannels>(_ => {
				Sender.Tell(_ChannelActors.Select(kv => kv.Key).ToArray());
			});
			this.DataContext = dataContext;

		}

		public IChannelConfigurationContext DataContext { get; }
		public ILoggingAdapter Logger { get; }

		private bool GetChannelActor(JoinChannel msg) {

			if (_ChannelActors.ContainsKey(msg.ChannelName)) {
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return false;
			}

			var config = DataContext.GetConfigurationForChannel(msg.ChannelName);

			var child = Context.ActorOf(ChannelActor.Props(config), $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

			return true;

		}

	}
}
