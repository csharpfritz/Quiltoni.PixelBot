using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quiltoni.PixelBot.GiveawayGame;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class GiveawayGameCommand : IBotCommand, IBotListensToMesages
	{
		public bool Enabled { get; } = true;

		public string CommandText => "giveaway";

		public GiveawayGame.GiveawayGame Game { get; }

		public GiveawayGameCommand(GiveawayGame.GiveawayGame game) {

			this.Game = game;

		}

		public ChatUser ChatUser { get; private set; }

		public void Execute(ChatCommand cmd, IChatService twitch) {

			this.ChatUser = cmd.ChatMessage.AsChatUser();

			if (!cmd.ArgumentsAsList.Any()) {
				Game.Help(twitch, this);
			}

			switch (cmd.ArgumentsAsList[0].ToLowerInvariant()) {

				case "help":
					Game.Help(twitch, this);
					break;
				case "open":
					Game.Open(twitch, this);
					break;
				case "start":
					Game.Start(twitch, this);
					break;

			}


		}

		public void MessageReceived(IChatService twitch) {

			if (Game.State != GiveawayGameState.Open) return;

			Game.EnterGiveaway(ChatUser.DisplayName);

		}
	}
}
