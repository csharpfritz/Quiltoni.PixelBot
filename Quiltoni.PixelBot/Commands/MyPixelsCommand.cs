using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class MyPixelsCommand : IBotCommand, IRequiresSheet
	{

		public string CommandText => Models.Currency.MyCommand;

		public ISheetProxy GoogleSheet { get; set; }

		public bool Enabled => true;

		public void Execute(ChatCommand command, IChatService twitch) {

			var pixels = GoogleSheet.FindPixelsForUser(command.ChatMessage.DisplayName);
			if (pixels == 0) {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} does not currently have any {Models.Currency.Name}");
			}
			else {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} currently has {pixels} {Models.Currency.Name}");
			}

		}
	}
}
