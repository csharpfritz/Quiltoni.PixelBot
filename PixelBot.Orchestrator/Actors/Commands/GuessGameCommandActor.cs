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
				switch (theCmd.ArgumentsAsList[0].ToLowerInvariant()) {
					case "open":
						_TheGame.Open(this, theCmd);
						break;
					case "reopen":
						_TheGame.Open(this, theCmd);
						break;
					case "help":
						_TheGame.Help(this, theCmd);
						break;
					case "close":
						_TheGame.Close(this, theCmd);
						break;
					case "mine":
						_TheGame.Mine(this, theCmd);
						break;
					case "end":
						_TheGame.Reset(this, theCmd);
						break;
					default:
						_TheGame.Guess(this, theCmd);
						break;
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
