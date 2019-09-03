using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.ApplicationServices;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.FollowerService;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{
	public class FollowerServiceActor : ReceiveActor
	{
		private TwitchAPI _API;
		private FollowerService _FollowerService;

		public FollowerServiceActor() {

			// Cheer 16300 clintonrocksmith 30/8/19 
			// Cheer 5000 fixterjake14 30/8/19 

			ConfigureFollowerService();

			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.Receive<TrackNewFollowers>(AddChannelToTrack);

		}

		private void AddChannelToTrack(TrackNewFollowers msg) {

			if (!_FollowerService.Enabled) {
				_FollowerService.Start();
			}

			_FollowerService.ChannelsToMonitor.Add(msg.ChannelName);
		}

		private void ConfigureFollowerService() {

			_API = new TwitchLib.Api.TwitchAPI();
			_API.Settings.ClientId = BotConfiguration.LoginName;
			_API.Settings.AccessToken = BotConfiguration.Password;
			_FollowerService = new TwitchLib.Api.Services.FollowerService(_API);
			_FollowerService.OnNewFollowersDetected += _FollowerService_OnNewFollowersDetected;

		}

		private void _FollowerService_OnNewFollowersDetected(object sender, OnNewFollowersDetectedArgs e) {

			// TODO: Notify the appopriate ChannelActor for the channel with a new follower

			var users = ConvertUserIdToUserName(e.NewFollowers.Select(f => f.ToUserId)).GetAwaiter().GetResult();

			foreach (var newFollower in e.NewFollowers) {

				var thisUser = users.First(u => u.Id == newFollower.ToUserId);
				ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, e.Channel, $"New Follower on channel ({e.Channel}): {thisUser.DisplayName}"));
				Context.Parent.Tell(e);

			}

		}

		private async Task<TwitchLib.Api.Helix.Models.Users.User[]> ConvertUserIdToUserName(IEnumerable<string> userIds) {

			var response = await _API.Helix.Users.GetUsersAsync(userIds.ToList());
			return response.Users;

		}

		public override void AroundPostStop() {

			if (_FollowerService.Enabled)
			{
				_FollowerService.Stop();
			}

			base.AroundPostStop();
		}

		public BotConfiguration BotConfiguration { get; } = Startup.BotConfiguration;

		public IActorRef ChatLogger { get; }
	}
}
