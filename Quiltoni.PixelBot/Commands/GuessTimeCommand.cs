using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stateless;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class GuessTimeCommand : IBotCommand
	{

		// Cheer 400 cpayette 05/3/19 
		// Cheer 100 roberttables 05/3/19 
		// Cheer 100 jamesmontemagno 05/3/19 

		public string CommandText => "guess";

		public GuessGame _TheGame { get; }

		private IConfiguration _Configuration;

		public bool Enabled => bool.Parse(_Configuration["PixelBot:Commands:GuessTimeCommand"]);

		public GuessTimeCommand(GuessGame game, IConfiguration config) {

			_TheGame = game;
			_Configuration = config;

		}

		public void Execute(ChatCommand command, IChatService twitch) {

			if (!command.ArgumentsAsList.Any()) {
				_TheGame.Help(twitch, command.AsGuessGameCommand());
				return;
			}

			try {
				switch (command.ArgumentsAsList[0].ToLowerInvariant()) {
					case "open":
						_TheGame.Open(twitch, command.AsGuessGameCommand());
						break;
					case "reopen":
						_TheGame.Open(twitch, command.AsGuessGameCommand());
						break;
					case "help":
						_TheGame.Help(twitch, command.AsGuessGameCommand());
						break;
					case "close":
						_TheGame.Close(twitch, command.AsGuessGameCommand());
						break;
					case "mine":
						_TheGame.Mine(twitch, command.AsGuessGameCommand());
						break;
					case "end":
						_TheGame.Reset(twitch, command.AsGuessGameCommand());
						break;
					default:
						_TheGame.Guess(twitch, command.AsGuessGameCommand());
						break;
				}
			} catch (InvalidOperationException) {
				twitch.WhisperMessage(command.ChatMessage.Username, "Invalid command...");
				_TheGame.Help(twitch, command.AsGuessGameCommand());
			}

		}

	}
}
