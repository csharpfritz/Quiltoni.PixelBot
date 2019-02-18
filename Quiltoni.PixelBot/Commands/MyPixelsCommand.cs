using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class MyPixelsCommand : IBotCommand, IRequiresSheet
	{

		public string CommandText => "mypixels";

		public GoogleSheetProxy GoogleSheet { get; set; }

		public void Execute(ChatCommand command, IChatService twitch) {

			var pixels = GoogleSheet.FindPixelsForUser(command.ChatMessage.DisplayName);
			if (pixels == 0) {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} does not currently have any pixels");
			}
			else {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} currently has {pixels} pixels");
			}

		}
	}
}
