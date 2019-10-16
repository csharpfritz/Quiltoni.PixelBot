using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Quiltoni.PixelBot.Commands;
using Stateless;

namespace Quiltoni.PixelBot.GiveawayGame
{

	public class GiveawayGame
	{
		private readonly GiveawayGameConfiguration _Config;
		private readonly StateMachine<GiveawayGameState, GiveawayGameTrigger> _machine;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setHelpTrigger;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setOpenTrigger;
		readonly StateMachine<GiveawayGameState, GiveawayGameTrigger>.TriggerWithParameters<IChatService, GiveawayGameCommand> _setStartTrigger;
		private readonly HashSet<string> _entrants = new HashSet<string>();

		private readonly string[] _BotEntrants = new[] { "quiltonibot", "streamelementsbot", "nightbot", "thefritzbot" };
		private readonly HashSet<string> _ExcludeChatters = new HashSet<string>() { "xMiniBergerx" };
		private readonly IHttpClientFactory _ClientFactory;
		private IChatService _Twitch;
		private string _TheWinner;

		public bool EnableCountdownTimer = true;

		public IEnumerable<string> Entrants { get { return _entrants.ToArray(); } }

		public GiveawayGame(IHttpClientFactory factory, IOptions<PixelBotConfig> config, bool enableCountdown = true) {

			// Cheer 400 pharewings 24/3/19 
			// Cheer 2500 devlead 28/3/19 
			// Cheer 100 perryatdigitalox 28/3/19 

			_Config = config.Value.GiveawayGame;

			_machine = new StateMachine<GiveawayGameState, GiveawayGameTrigger>(GiveawayGameState.Idle);
			_setHelpTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Help);
			_setOpenTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Open);
			_setStartTrigger = _machine.SetTriggerParameters<IChatService, GiveawayGameCommand>(GiveawayGameTrigger.Start);

			_machine.Configure(GiveawayGameState.Idle)
				.OnEntry(OnIdle)
				.InternalTransition(_setHelpTrigger, OnHelpCommand)
				.PermitDynamicIf(_setStartTrigger, WhenStarting, CanRestart)
				.PermitDynamicIf(_setOpenTrigger, WhenOpenFromIdle, CanOpen);

			_machine.Configure(GiveawayGameState.Open)
				.InternalTransition(_setHelpTrigger, OnHelpCommand)
				.Ignore(GiveawayGameTrigger.Open)
				.Permit(GiveawayGameTrigger.End, GiveawayGameState.Idle)
				.PermitDynamicIf(_setStartTrigger, WhenStarting, CanStart);

			_machine.Configure(GiveawayGameState.Running)
				.OnEntryAsync(TriggerRunningAnimation)
				.Ignore(GiveawayGameTrigger.Open)
				.InternalTransition(_setHelpTrigger, OnHelpCommand)
				.Permit(GiveawayGameTrigger.AnnounceWinner, GiveawayGameState.Reward);

			_machine.Configure(GiveawayGameState.Reward)
				.OnEntry(NotifyWinner)
				.Ignore(GiveawayGameTrigger.Open)
				.Permit(GiveawayGameTrigger.RewardCompleted, GiveawayGameState.Idle);


			_ClientFactory = factory;

		}

		private async Task TriggerRunningAnimation() {

			using (var client = _ClientFactory.CreateClient("giveaway")) {

				var response = await client.PostAsJsonAsync(_Config.RelayUrl, _entrants);
				_TheWinner = await response.Content.ReadAsStringAsync();

			}

			// TODO: Replace this with an event raised from the Razor Page and flows through to the statemachine
			if (EnableCountdownTimer) {
				await Task.Delay(9000 + 5000); // Adding 5 seconds to account for Twitch latency
			}
			_machine.Fire(GiveawayGameTrigger.AnnounceWinner);

		}

		private void NotifyWinner() {

			// Cheer 100 nikit9999 07/4/19 
			// Cheer 6003 tealoldman 07/4/19 

			_Twitch.BroadcastMessageOnChannel($"Congratulation @{_TheWinner} - you have won the raffle!");
			_TheWinner = string.Empty;
			_machine.Fire(GiveawayGameTrigger.RewardCompleted);

		}


		public void EnterGiveaway(string userName) {

			if (_entrants.Contains(userName)) return;

			// NOTE: We need a banlist for bots to exclude from the raffle
			if (_BotEntrants.Contains(userName)) return;

			// NOTE: We need a trolllist to exclude from the raffle - this should be maintained by Mods and Broadcaster
			if (_ExcludeChatters.Contains(userName)) return;

			this._entrants.Add(userName);

			using (var client = _ClientFactory.CreateClient("giveaway")) {

				client.PutAsJsonAsync(_Config.RelayUrl, new[] { userName }).GetAwaiter().GetResult();

			}

		}

		public GiveawayGameState State { get { return _machine.State; } }

		private bool CanOpen(IChatService twitch, GiveawayGameCommand cmd) {

			return (cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator);

		}

		private bool CanStart(IChatService twitch, GiveawayGameCommand cmd) {

			return cmd.ChatUser.IsBroadcaster;

		}

		private bool CanRestart(IChatService twitch, GiveawayGameCommand cmd) {

			return cmd.ChatUser.IsBroadcaster && _entrants.Any();

		}

		private GiveawayGameState WhenOpenFromIdle(IChatService twitch, GiveawayGameCommand cmd) {

			twitch.BroadcastMessageOnChannel("Now taking entries for the raffle...  please enter any text in chat to participate");

			if (_entrants.Any()) {

				using (var client = _ClientFactory.CreateClient("giveaway")) {

					client.PutAsJsonAsync(_Config.RelayUrl + "?id=1", _entrants.ToArray()).GetAwaiter().GetResult();

				}

			}

			return GiveawayGameState.Open;

		}

		private GiveawayGameState WhenStarting(IChatService twitch, GiveawayGameCommand cmd) {

			_Twitch = twitch;
			return GiveawayGameState.Running;

		}


		private void OnHelpCommand(IChatService chatService, GiveawayGameCommand cmd, StateMachine<GiveawayGameState, GiveawayGameTrigger>.Transition transition) {

			// Cheer 731 devlead 28/3/19 

			if (!(cmd.ChatUser.IsModerator || cmd.ChatUser.IsBroadcaster)) {
				chatService.WhisperMessage(cmd.ChatUser.Username, "You don't have access to this command");
				return;
			}

			switch (_machine.State) {

				case GiveawayGameState.Idle:
					chatService.WhisperMessage(cmd.ChatUser.Username, "You can open the raffle for entries by using the open verb like this:  !giveaway open.  Hide the animation, ending the game with !giveaway end");
					break;
				case GiveawayGameState.Open:
					if (cmd.ChatUser.IsModerator) {
						chatService.WhisperMessage(cmd.ChatUser.Username, "The broadcaster can start the raffle by using the start verb like this: !giveaway start, moderators and the broadcaster can exclude chatters from the giveaway with !giveaway exclude viewername, you can clear the raffle entrants with !giveaway clear, you can end the raffle without starting using !giveaway end");
					}
					else {
						chatService.WhisperMessage(cmd.ChatUser.Username, "You can start the raffle by using the start verb like this: !giveaway start, you can clear the raffle entrants with !giveaway clear, you can end the raffle without starting using !giveaway end");
					}
					break;
				case GiveawayGameState.Running:
					chatService.WhisperMessage(cmd.ChatUser.Username, "I will announce a winner shortly");
					break;

			}

		}

		private void OnIdle() {
			//throw new NotImplementedException();
		}


		public void Open(IChatService twitch, GiveawayGameCommand cmd) {

			if (State == GiveawayGameState.Reward || State == GiveawayGameState.Running) {
				twitch.WhisperMessage(cmd.ChatUser.Username, "Game is not ready to be re-opened.  Wait for the winner to be announced before executing !giveaway open");
				return;
			}

			_machine.Fire(_setOpenTrigger, twitch, cmd);

		}

		public void Start(IChatService twitch, GiveawayGameCommand cmd, int countdownSeconds = 5) {

			if (State == GiveawayGameState.Reward || State == GiveawayGameState.Running) {
				twitch.WhisperMessage(cmd.ChatUser.Username, "Game is not ready to be re-started.  Wait for the winner to be announced before executing !giveaway start");
				return;
			}

			if (EnableCountdownTimer) {
				for (var i = countdownSeconds; i > 0; i--) {

					Task.Delay(1000).GetAwaiter().GetResult();
					twitch.BroadcastMessageOnChannel($"Giveaway starting in {i} seconds...");

				}
			}


			_machine.FireAsync(_setStartTrigger, twitch, cmd);

		}


		public void Help(IChatService twitch, GiveawayGameCommand cmd) {

			_machine.Fire(_setHelpTrigger, twitch, cmd);

		}

		public void Exclude(IChatService twitch, GiveawayGameCommand cmd) {

			// Cheer 100 nicklarsen 05/4/19 
			// Cheer 300 Mike_from_PlayrGG 05/4/19 

			if (State != GiveawayGameState.Open) return;

			if (!(cmd.ChatUser.IsBroadcaster || cmd.ChatUser.IsModerator)) return;

			// NOTE: May want to persist this group to disk

			_entrants.Remove(cmd.Arguments.Skip(1).First());
			_ExcludeChatters.Add(cmd.Arguments.Skip(1).First());

		}

		public void ClearEntrants() {

			_entrants.Clear();

		}

		public async Task End() {

			if (!(_machine.State == GiveawayGameState.Idle || _machine.State == GiveawayGameState.Open)) return;

			_entrants.Clear();

			using (var client = _ClientFactory.CreateClient("giveaway")) {

				var response = await client.GetAsync(_Config.RelayUrl);

			}

			_machine.Fire(GiveawayGameTrigger.End);


		}

	}
	 
}
