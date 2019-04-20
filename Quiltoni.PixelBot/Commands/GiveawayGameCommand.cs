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

		public static readonly Dictionary<string, Action<GiveawayGame.GiveawayGame, IChatService, GiveawayGameCommand>> _Verbs =
			new Dictionary<string, Action<GiveawayGame.GiveawayGame, IChatService, GiveawayGameCommand>> {
				{ "help", (Game, twitch, cmd) => Game.Help(twitch, cmd) },
				{ "open", (Game, twitch, cmd) => Game.Open(twitch, cmd) },
				{ "clear", (Game, twitch, cmd) => Game.ClearEntrants() },
				{ "start", (Game, twitch, cmd) => Task.Run(() => Game.Start(twitch, cmd)) },
				{ "exclude", (Game, twitch, cmd) => Game.Exclude(twitch, cmd) },
				{ "end", (Game, twitch, cmd) => Game.End() }
			};


		public GiveawayGameCommand(GiveawayGame.GiveawayGame game) {

			this.Game = game;

		}

		public ChatUser ChatUser { get; set; }

		public IEnumerable<string> Arguments { get; private set; }

		public void Execute(ChatCommand cmd, IChatService twitch) {

			this.ChatUser = cmd.ChatMessage.AsChatUser();
			var theVerb = cmd.ArgumentsAsList.Any() ? cmd.ArgumentsAsList[0].ToLowerInvariant() : "";
			Arguments = cmd.ArgumentsAsList;

			if (!cmd.ArgumentsAsList.Any() || !_Verbs.ContainsKey(theVerb)) {
				Game.Help(twitch, this);
				return;
			}

			_Verbs[theVerb](Game, twitch, this);

		}

		public void MessageReceived(IChatService twitch, string userName) {

			if (Game.State != GiveawayGameState.Open) return;

			Game.EnterGiveaway(userName);

		}
	}
}
