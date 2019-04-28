using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;
using Stateless.Graph;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Games.GuessGame
{

	public class TheGame
	{

		private readonly StateMachine<GuessGameState, GuessGameTrigger> _machine;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setHelpTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setGuessTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setOpenTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setCloseTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setMineTrigger;
		readonly StateMachine<GuessGameState, GuessGameTrigger>.TriggerWithParameters<IChatService, ChatCommand> _setResetTrigger;
		private readonly Dictionary<string, TimeSpan> _guesses = new Dictionary<string, TimeSpan>();

		public TheGame(GuessGameState initialState = GuessGameState.NotStarted) {
			_machine = new StateMachine<GuessGameState, GuessGameTrigger>(initialState);
			_setHelpTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.Help);
			_setGuessTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.TakeGuess);
			_setOpenTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.Open);
			_setCloseTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.Close);
			_setMineTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.Mine);
			_setResetTrigger = _machine.SetTriggerParameters<IChatService, ChatCommand>(GuessGameTrigger.Reset);

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

		private GuessGameState WhenResetting(IChatService chatService, ChatCommand cmd) {

			if (cmd.ArgumentsAsList.Count < 2) throw new InvalidOperationException();

			if (!TimeSpan.TryParseExact(cmd.ArgumentsAsList[1], "m\\:ss", null, out TimeSpan time)) return GuessGameState.NotStarted;
			if (_guesses.Any(kv => kv.Value == time)) {

				var found = _guesses.FirstOrDefault(kv => kv.Value == time);
				chatService.BroadcastMessageOnChannel($"WINNER!!! - Congratulations {found.Key} - you have won!");

			}
			else {

				var closest = _guesses.DefaultIfEmpty().Select(g => new { user = g.Key, seconds = g.Value.TotalSeconds, close = Math.Abs(g.Value.TotalSeconds - time.TotalSeconds) })
						.OrderBy(g => g.close)
						.First();

				chatService.BroadcastMessageOnChannel($"No winners THIS time, but {closest.user} was closest at just {closest.close} seconds off!");

			}
			return GuessGameState.NotStarted;
		}
		private GuessGameState WhenOpeningFromClosed(IChatService chatService, ChatCommand cmd) {
			chatService.BroadcastMessageOnChannel("Now taking guesses again!  You may log a new guess or change your guess now!");
			return GuessGameState.OpenTakingGuesses;
		}
		private GuessGameState WhenClosingFromTakingGuesses(IChatService chatService, ChatCommand cmd) {
			chatService.BroadcastMessageOnChannel($"No more guesses...  the race is about to start with {_guesses.Count} guesses from {_guesses.DefaultIfEmpty().Min(kv => kv.Value).ToString()} to {_guesses.DefaultIfEmpty().Max(kv => kv.Value).ToString()}");
			return GuessGameState.GuessesClosed;
		}
		private GuessGameState WhenOpeningFromNotStarted(IChatService chatService, ChatCommand cmd) {
			chatService.BroadcastMessageOnChannel("Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.");
			return GuessGameState.OpenTakingGuesses;
		}
		private bool CanClose(IChatService service, ChatCommand cmd) {
			return cmd.IsBroadcaster || cmd.IsModerator;
		}
		private bool CanReset(IChatService service, ChatCommand cmd) {
			return cmd.IsBroadcaster || cmd.IsModerator;
		}
		private bool CanOpen(IChatService service, ChatCommand cmd) {
			return cmd.IsBroadcaster || cmd.IsModerator;
		}
		private void OnNotStartedAction() {
			_guesses.Clear();
		}
		private void OnMineCommand(IChatService chatService, ChatCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition) {
			switch (transition.Destination) {
				case GuessGameState.NotStarted:
					break;
				case GuessGameState.OpenTakingGuesses:
					chatService.BroadcastMessageOnChannel(_guesses.Any(kv => kv.Key == cmd.Username)
							? $"{cmd.Username} guessed {_guesses[cmd.Username].ToString()}"
							: $"{cmd.Username} has not guessed yet!");
					break;
				case GuessGameState.GuessesClosed:
					chatService.BroadcastMessageOnChannel(_guesses.Any(kv => kv.Key == cmd.Username)
							? $"{cmd.Username} guessed {_guesses[cmd.Username].ToString()}"
							: $"{cmd.Username} has not guessed yet!");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private void OnTakeGuessCommand(IChatService chatService, ChatCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition) {
			if (//cmd.ArgumentsAsList[0].Contains("-") ||
					!TimeSpan.TryParseExact(cmd.ArgumentsAsList[0], "m\\:ss", null, out TimeSpan time)) {
				chatService.BroadcastMessageOnChannel($"Sorry {cmd.Username}, guesses are only accepted in the format !guess 1:23");
				return;
			}
			if (_guesses.Any(kv => kv.Value == time)) {
				var firstGuess = _guesses.First(kv => kv.Value == time);
				chatService.BroadcastMessageOnChannel($"Sorry {cmd.Username}, {firstGuess.Key} already guessed {time.ToString()}");
			}
			else if (_guesses.Any(kv => kv.Key == cmd.Username)) {
				_guesses[cmd.Username] = time;
			}
			else {
				_guesses.Add(cmd.Username, time);
			}
		}
		private void OnHelpCommand(IChatService chatService, ChatCommand cmd, StateMachine<GuessGameState, GuessGameTrigger>.Transition transition) {
			switch (transition.Destination) {
				case GuessGameState.NotStarted:
					chatService.WhisperMessage(cmd.DisplayName, "The time-guessing game is not currently running.  To open the game for guesses, execute !guess open");
					break;
				case GuessGameState.OpenTakingGuesses:
					if (cmd.IsBroadcaster || cmd.IsModerator) {
						chatService.WhisperMessage(cmd.Username, "The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23, Your last guess will stand, and you can check your guess with !guess mine, OR close the guesses with !guess close");
					}
					else {
						chatService.BroadcastMessageOnChannel("The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23  Your last guess will stand, and you can check your guess with !guess mine");
					}
					break;
				case GuessGameState.GuessesClosed:
					if (!cmd.IsBroadcaster && !cmd.IsModerator)
						chatService.BroadcastMessageOnChannel("The time-guessing game is currently CLOSED.  You can check your guess with !guess mine");
					else
						chatService.WhisperMessage(cmd.Username, "The time-guessing game is currently CLOSED.  You can check your guess with !guess mine, you can re-open for guessing with !guess open, and you can check for winners and reset the game with !guess end 1:23");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public GuessGameState CurrentState() {
			return _machine.State;
		}

		public string AsDiagram() {
			return UmlDotGraph.Format(_machine.GetInfo());
		}

		public int GuessCount() {
			return _guesses.Count;
		}

		private void SendGenericCommand(IChatService service, ChatCommand cmd) {
			if (service == null) throw new ArgumentException("IChatService should not be null");
			//var validator = new ChatCommandValidator();
			//var results = validator.Validate(cmd);
			//if (!results.IsValid) {
			//	foreach (var failure in results.Errors) {
			//		//todo: move to logger??
			//		Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
			//	}
			//	throw new ArgumentException("The requested command is invalid", nameof(cmd));
			//}

			if (cmd.ArgumentsAsList.Count == 0) {
				_machine.Fire(_setHelpTrigger, service, cmd);
				return;
			}
			//todo: choice based on argumentlist instead of called method??
			switch (cmd.ArgumentsAsList[0]) {
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

		public void Help(IChatService service, ChatCommand cmd) {
			cmd.ArgumentsAsList = new List<string>() { "help" };
			SendGenericCommand(service, cmd);
		}

		public void Open(IChatService service, ChatCommand cmd) {
			cmd.ArgumentsAsList = new List<string>() { "open" };
			SendGenericCommand(service, cmd);
		}

		public void Close(IChatService service, ChatCommand cmd) {
			cmd.ArgumentsAsList = new List<string>() { "close" };
			SendGenericCommand(service, cmd);
		}

		public void Reset(IChatService service, ChatCommand cmd) {
			//cmd.ArgumentsAsList = new List<string>() { "reset" };
			SendGenericCommand(service, cmd);
		}

		public void Guess(IChatService service, ChatCommand cmd) {
			SendGenericCommand(service, cmd);
		}

		public void Mine(IChatService service, ChatCommand cmd) {
			cmd.ArgumentsAsList = new List<string>() { "mine" };
			SendGenericCommand(service, cmd);
		}
	}

}
