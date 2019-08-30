using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;
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
			ChatLogger.Tell(new MSG.ChatLogMessage(LogLevel.Information, e.Channel, $"New Follower on channel ({e.Channel}): {e.NewFollowers.First().}" + args.ChatMessage.Message));
			Context.Parent.Tell(e);

		}

		private async Task<string> ConvertUserIdToUserName(string userId) {

			var response = await _API.Helix.Users.GetUsersAsync(new List<string>() { userId });
			return response.Users.First().DisplayName;

		}

		public override void AroundPreStart() {

			_FollowerService.Start();
			
			base.AroundPreStart();
		}

		public override void AroundPostStop() {

			_FollowerService.Stop();

			base.AroundPostStop();
		}

		public BotConfiguration BotConfiguration { get; } = Startup.BotConfiguration;

		public IActorRef ChatLogger { get; }
	}
}
