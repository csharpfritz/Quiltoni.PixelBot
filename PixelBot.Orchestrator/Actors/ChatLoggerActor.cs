using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using PixelBot.Orchestrator.Services;
using PixelBot.ResolverActors;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{
	public class ChatLoggerActor : ReceiveActor
	{

		public const string Name = "chatlogger";
		public static string Path;

		public ChatLoggerActor()
		{
			this.ChatLogger = this.RequestService<IHubContext<LoggerHub, IChatLogger>>(); ;
			Path = Context.Self.Path.ToString();

			ReceiveAsync<ChatLogMessage>(async m => await ChatLogger.Clients.All.LogMessage(m.LogLevel.ToString(), "#" + m.Channel + ": " + m.Message));

		}

		public IHubContext<LoggerHub, IChatLogger> ChatLogger { get; }

		public static IActorRef Create()
		{
			var props = Akka.Actor.Props.Create<ChatLoggerActor>();
			return Context.ActorOf(props, Name);

		}

	}
}
