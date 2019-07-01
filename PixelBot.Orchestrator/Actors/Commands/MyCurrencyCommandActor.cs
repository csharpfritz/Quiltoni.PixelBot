using Akka.Actor;
using MSG = Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;
using DOMAIN = Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Actors.Commands
{
	public class MyCurrencyCommandActor : ReceiveActor, IBotCommandActor
	{
		
		public MyCurrencyCommandActor(DOMAIN.ChannelConfiguration config) {

			this.Config = config;

			Receive<OnChatCommandReceivedArgs>(args => Execute(args));

		}

		public string CommandText => Config.Currency.MyCommand.ToLowerInvariant();

		private void Execute(OnChatCommandReceivedArgs args) {

			Sender.Tell(new MSG.Currency.MyCurrencyMessage(args.Command.ChatMessage.Username));

		}

		public DOMAIN.ChannelConfiguration Config { get; }

		public static IActorRef CreateActor(DOMAIN.ChannelConfiguration config) {

			var props = Akka.Actor.Props.Create<MyCurrencyCommandActor>(config);
			return Context.ActorOf(props);

		}


	}

}
