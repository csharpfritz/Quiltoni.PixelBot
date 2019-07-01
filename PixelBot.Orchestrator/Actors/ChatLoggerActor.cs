using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{
	public class ChatLoggerActor : ReceiveActor
	{

		public const string Name = "chatlogger";
		public static string Path;

		public ChatLoggerActor(IHubContext<LoggerHub, IChatLogger> chatContext) {

			this.ChatLogger = chatContext;

			Path = Context.Self.Path.ToString();

			ReceiveAsync<ChatLogMessage>(async m => await ChatLogger.Clients.All.LogMessage(m.LogLevel.ToString(), "#" + m.Channel + ": " + m.Message));

		}

		public IHubContext<LoggerHub, IChatLogger> ChatLogger { get; }

		public static IActorRef Create(IHubContext<LoggerHub, IChatLogger> hubContext) {

			var props = Akka.Actor.Props.Create<ChatLoggerActor>(hubContext);
			return Context.ActorOf(props, Name);

		}

	}
}
