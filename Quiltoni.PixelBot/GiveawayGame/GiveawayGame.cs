using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Quiltoni.PixelBot.Commands;
using Stateless;

namespace Quiltoni.PixelBot.GiveawayGame
{

	public class GiveawayGame
	{

		private readonly StateMachine<GiveawayGameState, GiveawayGameTrigger> _machine;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setHelpTrigger;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setOpenTrigger;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setStartTrigger;

		private readonly List<string> _entrants = new List<string>();

		private readonly string[] _BlackListEntrants = new[] { "quiltonibot", "streamelementsbot", "nightbot", "thefritzbot" };


		public GiveawayGame() {

			// Cheer 400 pharewings 24/3/19 
			// Cheer 2500 devlead 28/3/19 
			// Cheer 100 perryatdigitalox 28/3/19 

			_machine = new StateMachine<GiveawayGameState, GiveawayGameTrigger>(GiveawayGameState.Idle);
			_setHelpTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Help);
			_setOpenTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Open);
			_setStartTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Start);

			_machine.Configure(GiveawayGameState.Idle)
				.OnEntry(OnIdle)
				.InternalTransition(_setHelpTrigger, OnHelpCommand)
				.PermitDynamicIf(_setOpenTrigger, WhenOpenFromIdle, CanOpen);

			_machine.Configure(GiveawayGameState.Open)
				.InternalTransition(_setHelpTrigger, OnHelpCommand)
				.PermitDynamicIf(_setStartTrigger, WhenStarting, CanStart);

			_machine.Configure(GiveawayGameState.Running)
				.OnEntryAsync(TriggerRunningAnimation)
				.InternalTransition(_setHelpTrigger, OnHelpCommand);


		}

		private async Task TriggerRunningAnimation() {

			using (var client = new HttpClient()) {

				await client.PostAsync("https://quiltonirelay.azurewebsites.net/api/GiveawayGame", _entrants.ToArray(), new JsonMediaTypeFormatter());

			}

		}

		internal void EnterGiveaway(string displayName) {

			if (_entrants.Contains(displayName)) return;

			// NOTE: We need a blacklist for bots to exclude from the raffle
			if (_BlackListEntrants.Contains(displayName.ToLowerInvariant())) return;

			this._entrants.Add(displayName);

		}

		public GiveawayGameState State { get { return _machine.State; } }

		private bool CanOpen(IChatService twitch, GiveawayGameCommand cmd) {

			return (cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator);

		}

		private bool CanStart(IChatService twitch, GiveawayGameCommand cmd ) {

			return cmd.ChatUser.IsBroadcaster;

		}

		private GiveawayGameState WhenOpenFromIdle(IChatService twitch, GiveawayGameCommand cmd) {

			twitch.BroadcastMessageOnChannel("Now taking entries for the raffle...  please enter any text in chat to participate");
			return GiveawayGameState.Open;

		}

		private GiveawayGameState WhenStarting(IChatService twitch, GiveawayGameCommand cmd) {

			return GiveawayGameState.Running;

		}


		private void OnHelpCommand(IChatService chatService, GiveawayGameCommand cmd, StateMachine<GiveawayGameState, GiveawayGameTrigger>.Transition transition) {

			// Cheer 731 devlead 28/3/19 

			if (!(cmd.ChatUser.IsModerator || cmd.ChatUser.IsBroadcaster)) {
				chatService.WhisperMessage(cmd.ChatUser.DisplayName, "You don't have access to this command");
				return;
			}

			switch (_machine.State) {

				case GiveawayGameState.Idle:
					chatService.WhisperMessage(cmd.ChatUser.DisplayName, "You can open the raffle for entries by using the open verb like this:  !giveaway open");
					break;
				case GiveawayGameState.Open:
					if (cmd.ChatUser.IsModerator) {
						chatService.WhisperMessage(cmd.ChatUser.DisplayName, "The broadcaster can start the raffle by using the start verb like this: !giveaway start");
					}
					else {
						chatService.WhisperMessage(cmd.ChatUser.DisplayName, "You can start the raffle by using the start verb like this: !giveaway start");
					}
					break;
				case GiveawayGameState.Running:
					chatService.WhisperMessage(cmd.ChatUser.DisplayName, "I will announce a winner shortly");
					break;

			}

		}

		private void OnIdle() {
			//throw new NotImplementedException();
		}


		public void Open(IChatService twitch, GiveawayGameCommand cmd) {

			_machine.Fire(_setOpenTrigger, twitch, cmd);

		}

		public void Start(IChatService twitch, GiveawayGameCommand cmd) {

			_machine.Fire(_setStartTrigger, twitch, cmd);

		}


		public void Help(IChatService twitch, GiveawayGameCommand cmd) {

			_machine.Fire(_setHelpTrigger, twitch, cmd);

		}

	}

}
