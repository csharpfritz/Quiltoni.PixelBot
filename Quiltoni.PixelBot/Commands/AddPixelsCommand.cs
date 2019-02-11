using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class AddPixelsCommand : IBotCommand, IRequiresSheet
	{

		public ISheetProxy GoogleSheet { get; set; }


		public string CommandText => "add";

		public bool Enabled => true;

		public void Execute(ChatCommand command, IChatService twitch) {

			if (!Validate(command, twitch)) return;

			var userName = command.ArgumentsAsList[0].Trim();

			if (userName == "all") {
				GoogleSheet.AddPixelsForChatters(command.ChatMessage.Channel, int.Parse(command.ArgumentsAsList[1]), command.ChatMessage.DisplayName);
			} else {
				GoogleSheet.AddPixelsForUser(command.ArgumentsAsList[0].Trim(), int.Parse(command.ArgumentsAsList[1]), command.ChatMessage.DisplayName);
			}

		}

		private bool Validate(ChatCommand command, IChatService twitch) {

			// Only broadcasters and moderators are allowed to add pixels
			if (!(command.ChatMessage.IsBroadcaster || command.ChatMessage.IsModerator)) 
			{
				twitch.BroadcastMessageOnChannel("Only moderators can execute the !add command");
				return false;
			}

			if (command.ArgumentsAsList.Count != 2) {
				twitch.WhisperMessage(command.ChatMessage.DisplayName, $"Invalid format to add {Models.Currency.Name}.  \"!add username {Models.Currency.Name}ToAdd\"");
				return false;
			}
			else if (!int.TryParse(command.ArgumentsAsList[1], out int pixels)) {
				twitch.WhisperMessage(command.ChatMessage.DisplayName, $"Invalid format to add {Models.Currency.Name}.  \"!add username {Models.Currency.Name}ToAdd\"");
				return false;
			}

			return true;

		}

	}

}
