using System.Diagnostics;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class NewMessageActor : ReceiveActor
	{

		public NewMessageActor(ChannelConfiguration config) {

			this.Configuration = config;

			this.Receive<OnMessageReceivedArgs>((args) => {

				Debug.WriteLine(args.ChatMessage.Username + ": " + args.ChatMessage.Message);

			});


		}

		public ChannelConfiguration Configuration { get; }
	}

}
