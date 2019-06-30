using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class MyPixelsCommand : IBotCommand, IRequiresSheet
	{

		public MyPixelsCommand(IOptions<PixelBotConfig> config) {
			CommandText = config.Value.Currency.MyCommand;
			_CurrencyName = config.Value.Currency.Name;
		}

		public string CommandText { get; private set; }

		private string _CurrencyName;

		public ISheetProxy GoogleSheet { get; set; }

		public bool Enabled => true;

		public void Execute(ChatCommand command, IChatService twitch) {

			var pixels = GoogleSheet.FindPixelsForUser(command.ChatMessage.DisplayName);
			if (pixels == 0) {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} does not currently have any {_CurrencyName}");
			}
			else {
				twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} currently has {pixels} {_CurrencyName}");
			}

		}
	}
}
