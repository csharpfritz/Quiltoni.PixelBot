using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Extensibility;
using MSG = Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class NewMessageActor : ReceiveActor
	{

		public NewMessageActor(ChannelConfiguration config, IEnumerable<IFeature> features) {

			this.Configuration = config;

			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			// TODO:  This should be a static method on the ChannelActor object
			_ChannelActor = Context.ActorSelection($"/user/channelmanager/channel_{config.ChannelName}");

			// Cheer 100 nothing_else_matters 05/07/19 
			// Cheer 200 cpayette 05/07/19 
			// Cheer 300 pakmanjr 05/07/19 
			this.Features = features.ToArray();
			foreach (var f in Features) {
				f.BroadcastMessage = (msg) => _ChannelActor.Tell(new MSG.BroadcastMessage(msg));
				f.WhisperMessage = (user, msg) => _ChannelActor.Tell(new MSG.WhisperMessage(user, msg));
			}

			this.Receive<OnMessageReceivedArgs>((args) => {

				Debug.WriteLine(args.ChatMessage.DisplayName + ": " + args.ChatMessage.Message);
				ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, Configuration.ChannelName, args.ChatMessage.DisplayName + ": " + args.ChatMessage.Message));

				foreach (var f in Features.Where(f => f.IsEnabled)) {
					// TODO: Ensure we pass badges and emotes through to the feature
					f.FeatureTriggered(args.ChatMessage.DisplayName + ": " + args.ChatMessage.Message);
				}

			});
			this.Receive<MSG.GetFeatureForChannel>(f => {
				if (f.Channel != Configuration.ChannelName || !Features.Any(feature => feature.GetType() == f.FeatureType)) return;
				Sender.Tell(Features.First(feature => f.FeatureType == feature.GetType()));
			});

		}

		public ChannelConfiguration Configuration { get; }

		public IActorRef ChatLogger { get; }

		private readonly ActorSelection _ChannelActor;

		public IFeature[] Features { get; private set; }

	}

}
