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
		public const string Name = "channelmanager";

		public static IActorRef Create(
			ActorSystem system, 
			IChannelConfigurationContext dataContext, 
			IHubContext<LoggerHub, IChatLogger> chatLogger) {

			var props = Props.Create<ChannelManagerActor>(dataContext, chatLogger);
			return system.ActorOf(props, Name);

		}

		public ChannelManagerActor(IChannelConfigurationContext dataContext, IHubContext<LoggerHub, IChatLogger> chatLogger) {

			Logger = Context.GetLogger();

			_ChatLogger = ChatLoggerActor.Create(chatLogger);

			Receive<JoinChannel>(this.GetChannelActor);
			Receive<ReportCurrentChannels>(_ => {
				 Sender.Tell(_ChannelActors.Select(kv => kv.Key).ToArray());
			});
			//Receive<GetFeatureForChannel>(async f => {
			//	var theChannelActor = _ChannelActors[f.Channel];
			//	Sender.Tell(await theChannelActor.Ask(new GetFeatureFromChannel(f.FeatureType)));
			//});
			this.DataContext = dataContext;

		}

		public IChannelConfigurationContext DataContext { get; }
		public ILoggingAdapter Logger { get; }

		private bool GetChannelActor(JoinChannel msg) {

			if (string.IsNullOrEmpty(msg.ChannelName)) return false;

			if (_ChannelActors.ContainsKey(msg.ChannelName)) {
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return false;
			}

			var config = DataContext.GetConfigurationForChannel(msg.ChannelName);

			var child = Context.ActorOf(ChannelActor.Props(config), $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

			return true;

		}

		public ChannelActor this[string channelName] {
			get { return _ChannelActors[channelName] as ChannelActor; }
		}

	}
}
