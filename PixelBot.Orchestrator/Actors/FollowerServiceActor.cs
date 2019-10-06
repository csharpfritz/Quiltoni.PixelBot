using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

    public class FollowerServiceActor : ReceiveActor
    {
        public const string TWITCH_SECRET = "MYSECRET";
        private readonly IHubContext<UserActivityHub, IUserActivityClient> _FollowHubContext;
        private readonly HttpClient _Client;
        private static readonly HashSet<string> _FollowerChannels = new HashSet<string>();

        public FollowerServiceActor(IHubContext<UserActivityHub,IUserActivityClient> followHubContext, IHttpClientFactory httpClientFactory)
        {
            
			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.ReceiveAsync<TrackNewFollowers>(AddChannelToTrack);

			_FollowHubContext = followHubContext;
            this._Client = httpClientFactory.CreateClient("TwitchWebHook");

        }

        private async Task AddChannelToTrack(TrackNewFollowers arg)
        {
            
            // Check if the channel is already subscribed to with the WebHook API
            if (_FollowerChannels.Contains(arg.ChannelName)) return;

            await SubscribeToTwitchWebhook(arg.ChannelName);

            _FollowerChannels.Add(arg.ChannelName);

            ChatLogger.Tell(new ChatLogMessage(LogLevel.Debug, "- global -", $"Now tracking followers for channel {arg.ChannelName}"));

        }

        private async Task SubscribeToTwitchWebhook(string channelName)
        {
           
#if DEBUG           
            var leaseInSeconds = 300;
#else
            var leaseInSeconds = 3600;
#endif            

            var payload = new TwitchWebhookSubscriptionPayload {
                callback = "",
                mode = "subscribe",
                topic = "https://api.twitch.tv/helix/users/follows",
                lease_seconds = leaseInSeconds,
                secret = TWITCH_SECRET
            };
            var stringPayload = JsonConvert.SerializeObject(new {hub=payload});

            await _Client.PostAsync("hub", new StringContent(stringPayload));

        }

        public IActorRef ChatLogger { get; }

        public override void AroundPostStop()  {

            _Client.Dispose();

            base.AroundPostStop();

        }

        public class TwitchWebhookSubscriptionPayload {

            public string callback { get; set; }

            public string mode { get; set; }

            public string topic { get; set; }

            public int lease_seconds { get; set; }

            public string secret { get; set; }

        }


    }

}