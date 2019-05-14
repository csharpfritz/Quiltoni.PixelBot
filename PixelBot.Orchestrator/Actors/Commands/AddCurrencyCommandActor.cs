using System;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Data;
using MSG = Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using DOMAIN = Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Actors.Commands
{
	public class AddCurrencyCommandActor : ReceiveActor, IBotCommandActor
	{

		public AddCurrencyCommandActor(DOMAIN.ChannelConfiguration config, ICurrencyRepository currencyRepository) {

			GoogleSheet = currencyRepository;
			this.Config = config;

			Receive<OnChatCommandReceivedArgs>(args => Execute(args));

		}

		public ICurrencyRepository GoogleSheet { get; set; }
		public DOMAIN.ChannelConfiguration Config { get; }

		public string CommandText => "add";

		public bool Enabled => true;

		public void Execute(OnChatCommandReceivedArgs args) {

			if (!Validate(args.Command)) return;

			var userName = args.Command.ArgumentsAsList[0].Trim();

			if (userName == "all") {
				Sender.Tell(new MSG.Currency.AddCurrencyMessage("#" + args.Command.ChatMessage.Channel, int.Parse(args.Command.ArgumentsAsList[1]), args.Command.ChatMessage.DisplayName));
				//GoogleSheet.AddForChatters(args.Command.ChatMessage.Channel, int.Parse(args.Command.ArgumentsAsList[1]), args.Command.ChatMessage.DisplayName);
			}
			else {
				Sender.Tell(new MSG.Currency.AddCurrencyMessage(args.Command.ArgumentsAsList[0].Trim(), int.Parse(args.Command.ArgumentsAsList[1]), args.Command.ChatMessage.DisplayName));
				//GoogleSheet.AddForUser(args.Command.ArgumentsAsList[0].Trim(), int.Parse(args.Command.ArgumentsAsList[1]), args.Command.ChatMessage.DisplayName);
			}

		}

		private bool Validate(ChatCommand command) {

			// Only broadcasters and moderators are allowed to add pixels
			if (!(command.ChatMessage.IsBroadcaster || command.ChatMessage.IsModerator)) {
				Sender.Tell(new MSG.BroadcastMessage("Only moderators can execute the !add command"));
				return false;
			}

			if (command.ArgumentsAsList.Count != 2) {
				Sender.Tell(new MSG.WhisperMessage(command.ChatMessage.DisplayName, $"Invalid format to add {Config.Currency.Name}.  \"!add username {Config.Currency.Name}ToAdd\""));
				return false;
			}
			else if (!int.TryParse(command.ArgumentsAsList[1], out int pixels)) {
				Sender.Tell(new MSG.WhisperMessage(command.ChatMessage.DisplayName, $"Invalid format to add {Config.Currency.Name}.  \"!add username {Config.Currency.Name}ToAdd\""));
				return false;
			}

			return true;

		}


		public static IActorRef CreateActor(DOMAIN.ChannelConfiguration config, ICurrencyRepository currencyRepository) {

			var props = Akka.Actor.Props.Create<AddCurrencyCommandActor>(config, currencyRepository);
			return Context.ActorOf(props);

		}

	}
}
