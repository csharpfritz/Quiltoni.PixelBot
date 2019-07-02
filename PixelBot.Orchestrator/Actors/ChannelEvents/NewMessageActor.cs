using System;
using System.Diagnostics;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class NewMessageActor : ReceiveActor
	{

		public NewMessageActor(ChannelConfiguration config) {

			this.Configuration = config;

			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.Receive<OnMessageReceivedArgs>((args) => {

				Debug.WriteLine(args.ChatMessage.DisplayName + ": " + args.ChatMessage.Message);
				ChatLogger.Tell(new ChatLogMessage(LogLevel.Information, Configuration.ChannelName, args.ChatMessage.DisplayName + ": " + args.ChatMessage.Message));

			});


		}

		public ChannelConfiguration Configuration { get; }
		public IActorRef ChatLogger { get; }
	}

}
