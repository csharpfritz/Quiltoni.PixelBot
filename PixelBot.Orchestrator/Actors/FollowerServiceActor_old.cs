using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.FollowerService;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{
	public class FollowerServiceActor_old : ReceiveActor
	{
		private TwitchAPI _API;
		private FollowerService _FollowerService;
		private Dictionary<string, (string ChannelName, DateTime StartTime)> _Channels = new Dictionary<string, (string, DateTime)>();
		private readonly IHubContext<UserActivityHub, IUserActivityClient> _FollowHubContext;

		public FollowerServiceActor_old(IHubContext<UserActivityHub, IUserActivityClient> followHubContext)
		{

			// Cheer 16300 clintonrocksmith 30/8/19 
			// Cheer 5000 fixterjake14 30/8/19 

			ConfigureFollowerService();

			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.ReceiveAsync<TrackNewFollowers>(AddChannelToTrack);
			_FollowHubContext = followHubContext;

		}

		private async Task AddChannelToTrack(TrackNewFollowers msg)
		{

			var thisUser = await ConvertUserNameToUserId(new[] { msg.ChannelName });
			_Channels.Add(thisUser.First().Id, (msg.ChannelName, DateTime.UtcNow));
			if (_FollowerService.ChannelsToMonitor == null)
			{
				_FollowerService.SetChannelsByName(new List<string> { msg.ChannelName });
			}
			else
			{
				_FollowerService.ChannelsToMonitor.Add(msg.ChannelName);
			}

			if (!_FollowerService.Enabled)
			{
				_FollowerService.Start();
			}

		}

		private void ConfigureFollowerService()
		{

			_API = new TwitchLib.Api.TwitchAPI();
			_API.Settings.ClientId = BotConfiguration.LoginName;
			_API.Settings.AccessToken = BotConfiguration.Password;
			_FollowerService = new TwitchLib.Api.Services.FollowerService(_API, 5);
			_FollowerService.OnServiceTick += _FollowerService_OnServiceTick;
			_FollowerService.OnNewFollowersDetected += _FollowerService_OnNewFollowersDetected;

		}

		private void _FollowerService_OnServiceTick(object sender, OnServiceTickArgs e)
		{

			ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, "- global -", $"Follower Service Tick"));

		}

		private void _FollowerService_OnNewFollowersDetected(object sender, OnNewFollowersDetectedArgs e)
		{

			// TODO: Notify the appopriate ChannelActor for the channel with a new follower
			ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, "- global -", $"New Follower Detected"));

			var filteredFollowers = e.NewFollowers.Where(f => _Channels[f.ToUserId].StartTime < f.FollowedAt);

			if (!filteredFollowers.Any()) return;

			var users = ConvertUserIdToUserName(filteredFollowers.Select(f => f.FromUserId)).GetAwaiter().GetResult();

			foreach (var newFollower in filteredFollowers)
			{

				var thisUser = users.First(u => u.Id == newFollower.FromUserId);
				ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, e.Channel, $"New Follower on channel ({e.Channel}): {thisUser.DisplayName}"));

				// TODO: Trigger features listening for Follower events

				// TODO: Notify the ChannelActor for this channel
				//Context.Parent.Tell(e); 



				_FollowHubContext.Clients.Group(_Channels[newFollower.ToUserId].ChannelName).NewFollower(thisUser.DisplayName);

			}

		}

		private async Task<TwitchLib.Api.Helix.Models.Users.User[]> ConvertUserIdToUserName(IEnumerable<string> userIds)
		{

			var response = await _API.Helix.Users.GetUsersAsync(userIds.ToList());
			return response.Users;

		}

		private async Task<TwitchLib.Api.Helix.Models.Users.User[]> ConvertUserNameToUserId(IEnumerable<string> userNames)
		{

			var response = await _API.Helix.Users.GetUsersAsync(logins: userNames.ToList());
			return response.Users;

		}

		public override void AroundPostStop()
		{

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