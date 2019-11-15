using System;
using System.Collections.Generic;
using System.Linq;
using Quiltoni.PixelBot.Commands;
using Stateless;
using Stateless.Graph;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{

	public class GuessGame
	{
		private readonly StateMachine<GuessGameState, GuessGameTrigger> _machine;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setHelpTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setGuessTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setOpenTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setCloseTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setMineTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, GuessGameCommand> _setResetTrigger;
		private readonly Dictionary<string, TimeSpan> _guesses = new Dictionary<string, TimeSpan>();

		public GuessGame(GuessGameState initialState = GuessGameState.NotStarted)
		{
			_machine = new StateMachine<GuessGameState, GuessGameTrigger>(initialState);
			_setHelpTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.Help);
			_setGuessTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.TakeGuess);
			_setOpenTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.Open);
			_setCloseTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.Close);
			_setMineTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.Mine);
			_setResetTrigger = _machine.SetTriggerParameters<IChatService, GuessGameCommand>(GuessGameTrigger.Reset);

			_machine.Configure(GuessGameState.NotStarted)
					.OnEntry(OnNotStartedAction)
					.InternalTransition(_setHelpTrigger, OnHelpCommand)
					.InternalTransition(_setMineTrigger, OnMineCommand)
					.PermitDynamicIf(_setOpenTrigger, WhenOpeningFromNotStarted, CanOpen);

			_machine.Configure(GuessGameState.OpenTakingGuesses)
					.InternalTransition(_setGuessTrigger, OnTakeGuessCommand)
					.InternalTransition(_setMineTrigger, OnMineCommand)
					.InternalTransition(_setHelpTrigger, OnHelpCommand)
					.PermitDynamicIf(_setCloseTrigger, WhenClosingFromTakingGuesses, CanClose)
					;

			_machine.Configure(GuessGameState.GuessesClosed)
					.InternalTransition(_setMineTrigger, OnMineCommand)
					.InternalTransition(_setHelpTrigger, OnHelpCommand)
					.PermitDynamicIf(_setOpenTrigger, WhenOpeningFromClosed, CanClose)
					.PermitDynamicIf(_setResetTrigger, WhenResetting, CanReset);
		}

		private GuessGameState WhenResetting(IChatService twitch, GuessGameCommand cmd)
		{

			if (cmd.ArgumentsAsList.Count < 2) throw new InvalidOperationException();

			if (!TimeSpan.TryParseExact(cmd.ArgumentsAsList[1], "m\\:ss", null, out TimeSpan time)) return GuessGameState.NotStarted;
			if (_guesses.Any(kv => kv.Value == time))
			{

				var found = _guesses.FirstOrDefault(kv => kv.Value == time);
				twitch.BroadcastMessageOnChannel($"WINNER!!! - Congratulations {found.Key} - you have won!");

			}
			else
			{

				var closest = _guesses.DefaultIfEmpty().Select(g => new { user = g.Key, seconds = g.Value.TotalSeconds, close = Math.Abs(g.Value.TotalSeconds - time.TotalSeconds) })
						.OrderBy(g => g.close)
						.First();

				twitch.BroadcastMessageOnChannel($"No winners THIS time, but {closest.user} was closest at just {closest.close} seconds off!");

			}
			return GuessGameState.NotStarted;
		}
		private GuessGameState WhenOpeningFromClosed(IChatService twitch, GuessGameCommand cmd)
		{
			twitch.BroadcastMessageOnChannel("Now taking guesses again!  You may log a new guess or change your guess now!");
			return GuessGameState.OpenTakingGuesses;
		}
		private GuessGameState WhenClosingFromTakingGuesses(IChatService twitch, GuessGameCommand cmd)
		{
			twitch.BroadcastMessageOnChannel($"No more guesses...  the race is about to start with {_guesses.Count} guesses from {_guesses.DefaultIfEmpty().Min(kv => kv.Value).ToString()} to {_guesses.DefaultIfEmpty().Max(kv => kv.Value).ToString()}");
			return GuessGameState.GuessesClosed;
		}
		private GuessGameState WhenOpeningFromNotStarted(IChatService twitch, GuessGameCommand cmd)
		{
			twitch.BroadcastMessageOnChannel("Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.");
			return GuessGameState.OpenTakingGuesses;
		}
		private bool CanClose(IChatService service, GuessGameCommand cmd)
		{
			return cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator;
		}
		private bool CanReset(IChatService service, GuessGameCommand cmd)
		{
			return cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator;
		}
		private bool CanOpen(IChatService service, GuessGameCommand cmd)
		{
			return cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator;
		}
		private void OnNotStartedAction()
		{
			_guesses.Clear();
		}
		private void OnMineCommand(IChatService twitch, GuessGameCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition)
		{
			switch (transition.Destination)
			{
				case GuessGameState.NotStarted:
					break;
				case GuessGameState.OpenTakingGuesses:
					twitch.BroadcastMessageOnChannel(_guesses.Any(kv => kv.Key == cmd.ChatUser.Username)
							? $"{cmd.ChatUser.Username} guessed {_guesses[cmd.ChatUser.Username].ToString()}"
							: $"{cmd.ChatUser.Username} has not guessed yet!");
					break;
				case GuessGameState.GuessesClosed:
					twitch.BroadcastMessageOnChannel(_guesses.Any(kv => kv.Key == cmd.ChatUser.Username)
							? $"{cmd.ChatUser.Username} guessed {_guesses[cmd.ChatUser.Username].ToString()}"
							: $"{cmd.ChatUser.Username} has not guessed yet!");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void OnTakeGuessCommand(IChatService twitch, GuessGameCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition)
		{
			if (//cmd.ArgumentsAsList[0].Contains("-") ||
					!TimeSpan.TryParseExact(cmd.ArgumentsAsList[0], "m\\:ss", null, out TimeSpan time))
			{
				twitch.BroadcastMessageOnChannel($"Sorry {cmd.ChatUser.Username}, guesses are only accepted in the format !guess 1:23");
				return;
			}
			if (_guesses.Any(kv => kv.Value == time))
			{
				var firstGuess = _guesses.First(kv => kv.Value == time);
				twitch.BroadcastMessageOnChannel($"Sorry {cmd.ChatUser.Username}, {firstGuess.Key} already guessed {time.ToString()}");
			}
			else if (_guesses.Any(kv => kv.Key == cmd.ChatUser.Username))
			{
				_guesses[cmd.ChatUser.Username] = time;
			}
			else
			{
				_guesses.Add(cmd.ChatUser.Username, time);
			}
		}
		private void OnHelpCommand(IChatService twitch, GuessGameCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition)
		{
			switch (transition.Destination)
			{
				case GuessGameState.NotStarted:
					twitch.WhisperMessage(cmd.ChatUser.DisplayName, "The time-guessing game is not currently running.  To open the game for guesses, execute !guess open");
					break;
				case GuessGameState.OpenTakingGuesses:
					if (cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator)
					{
						twitch.WhisperMessage(cmd.ChatUser.Username, "The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23, Your last guess will stand, and you can check your guess with !guess mine, OR close the guesses with !guess close");
					}
					else
					{
						twitch.BroadcastMessageOnChannel("The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23  Your last guess will stand, and you can check your guess with !guess mine");
					}
					break;
				case GuessGameState.GuessesClosed:
					if (!cmd.ChatUser.IsBroadcaster && !cmd.ChatUser.IsModerator)
						twitch.BroadcastMessageOnChannel("The time-guessing game is currently CLOSED.  You can check your guess with !guess mine");
					else
						twitch.WhisperMessage(cmd.ChatUser.Username, "The time-guessing game is currently CLOSED.  You can check your guess with !guess mine, you can re-open for guessing with !guess open, and you can check for winners and reset the game with !guess end 1:23");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public GuessGameState CurrentState()
		{
			return _machine.State;
		}

		public string AsDiagram()
		{
			return UmlDotGraph.Format(_machine.GetInfo());
		}

		public int GuessCount()
		{
			return _guesses.Count;
		}

		private void SendGenericCommand(IChatService service, GuessGameCommand cmd)
		{
			if (service == null) throw new ArgumentException("IChatService should not be null");
			var validator = new GuessGameCommandValidator();
			var results = validator.Validate(cmd);
			if (!results.IsValid)
			{
				foreach (var failure in results.Errors)
				{
					//todo: move to logger??
					Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
				}
				throw new ArgumentException("The requested command is invalid", nameof(cmd));
			}

			if (cmd.ArgumentsAsList.Count == 0)
			{
				_machine.Fire(_setHelpTrigger, service, cmd);
				return;
			}
			//todo: choice based on argumentlist instead of called method??
			switch (cmd.ArgumentsAsList[0])
			{
				case "help":
					_machine.Fire(_setHelpTrigger, service, cmd);
					break;
				case "open":
					_machine.Fire(_setOpenTrigger, service, cmd);
					break;
				case "close":
					_machine.Fire(_setCloseTrigger, service, cmd);
					break;
				case "reset":
				case "end":
					_machine.Fire(_setResetTrigger, service, cmd);
					break;
				case "mine":
					_machine.Fire(_setMineTrigger, service, cmd);
					break;
				default:
					_machine.Fire(_setGuessTrigger, service, cmd);
					break;
			}
		}

		public void Help(IChatService service, GuessGameCommand cmd)
		{
			cmd.ArgumentsAsList = new List<string>() { "help" };
			SendGenericCommand(service, cmd);
		}

		public void Open(IChatService service, GuessGameCommand cmd)
		{
			cmd.ArgumentsAsList = new List<string>() { "open" };
			SendGenericCommand(service, cmd);
		}

		public void Close(IChatService service, GuessGameCommand cmd)
		{
			cmd.ArgumentsAsList = new List<string>() { "close" };
			SendGenericCommand(service, cmd);
		}

		public void Reset(IChatService service, GuessGameCommand cmd)
		{
			//cmd.ArgumentsAsList = new List<string>() { "reset" };
			SendGenericCommand(service, cmd);
		}

		public void Guess(IChatService service, GuessGameCommand cmd)
		{
			SendGenericCommand(service, cmd);
		}

		public void Mine(IChatService service, GuessGameCommand cmd)
		{
			cmd.ArgumentsAsList = new List<string>() { "mine" };
			SendGenericCommand(service, cmd);
		}
	}

}