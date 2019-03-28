using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class TealOldManCommand : IBotCommand
	{
		public bool Enabled { get; } = true;
		public string CommandText => "tealoldman";

		public void Execute(ChatCommand command, IChatService twitch) {

			twitch.BroadcastMessageOnChannel("TealOldMan is blowing up chat!  EVERYTHING is now blue!");

		}
	}
}
