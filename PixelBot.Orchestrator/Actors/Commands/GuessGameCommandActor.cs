using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using PixelBot.Games.GuessGame;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.Commands
{
	public class GuessGameCommandActor : ReceiveActor, IChatService, IBotCommandActor
	{

		// Cheer 400 cpayette 05/3/19 
		// Cheer 100 roberttables 05/3/19 
		// Cheer 100 jamesmontemagno 05/3/19 
		// Cheer 550 InaliTsusasi 28/4/19 
		// Cheer 110 TheMichaelJolley 28/4/19 
		// Cheer 156 cpayette 30/4/19 

		public string CommandText => "guess";

		public TheGame _TheGame { get; }

		public static readonly Dictionary<string, Action<TheGame, IChatService, ChatCommand>> _Actions = new Dictionary<string, Action<TheGame, IChatService, ChatCommand>> {
			{"open", (game, svc, cmd) => game.Open(svc, cmd)},
			{"reopen", (game, svc, cmd) => game.Open(svc, cmd)},
			{"help", (game, svc, cmd) => game.Help(svc, cmd)},
			{"close", (game, svc, cmd) => game.Close(svc, cmd)},
			{"mine", (game, svc, cmd) => game.Mine(svc, cmd)},
			{"end", (game, svc, cmd) => game.Reset(svc, cmd)}
		};

		public GuessGameCommandActor() {

			Receive<OnChatCommandReceivedArgs>(Execute);

		}

		public bool Execute(OnChatCommandReceivedArgs args) {

			var theCmd = new ChatCommand() {
				ArgumentsAsList = args.Command.ArgumentsAsList,
				DisplayName = args.Command.ChatMessage.DisplayName,
				IsBroadcaster = args.Command.ChatMessage.IsBroadcaster,
				IsModerator = args.Command.ChatMessage.IsModerator,
				Username = args.Command.ChatMessage.Username
			};

			if (!args.Command.ArgumentsAsList.Any()) {
				_TheGame.Help(this, theCmd);
				return true;
			}

			try {

				if (_Actions.ContainsKey(theCmd.ArgumentsAsList[0].ToLowerInvariant())) {

					_Actions[theCmd.ArgumentsAsList[0].ToLowerInvariant()](_TheGame, this, theCmd);

				} else {

					_TheGame.Guess(this, theCmd);

				}

			}
			catch (InvalidOperationException) {
				this.WhisperMessage(args.Command.ChatMessage.Username, "Invalid command...");
				_TheGame.Help(this, theCmd);
			}

			return true;

		}

		public void WhisperMessage(string username, string message) {

			Sender.Tell(new WhisperMessage(username, message));

		}

		public void BroadcastMessageOnChannel(string message) {

			Sender.Tell(new BroadcastMessage(message));

		}

		public List<string> ArgumentsAsList { get; set; }

	}
}
