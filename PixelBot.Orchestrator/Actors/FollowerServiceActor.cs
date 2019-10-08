using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
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

        public FollowerServiceActor(IHubContext<UserActivityHub,IUserActivityClient> followHubContext, 
            IHttpClientFactory httpClientFactory)
        {
            
			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.ReceiveAsync<TrackNewFollowers>(AddChannelToTrack);

			_FollowHubContext = followHubContext;
            this._Client = httpClientFactory.CreateClient("TwitchHelixApi");

        }

        private async Task AddChannelToTrack(TrackNewFollowers arg)
        {
            
            // Check if the channel is already subscribed to with the WebHook API
            if (_FollowerChannels.Contains(arg.ChannelName)) return;

            await SubscribeToTwitchWebhook(arg.ChannelId);

            _FollowerChannels.Add(arg.ChannelName);

            ChatLogger.Tell(new ChatLogMessage(Microsoft.Extensions.Logging.LogLevel.Debug, "- global -", $"Now tracking followers for channel {arg.ChannelName}"));

        }

        private async Task SubscribeToTwitchWebhook(string channelId)
        {

            // TODO: Renew leases when they expire
           
#if DEBUG           
            var leaseInSeconds = 300;
#else
            var leaseInSeconds = 3600;
#endif            

            var payload = new TwitchWebhookSubscriptionPayload {
                callback = "https://76c45e26.ngrok.io/api/follower",
                mode = "subscribe",
                topic = $"https://api.twitch.tv/helix/users/follows?first=1&to_id={channelId}",
                lease_seconds = leaseInSeconds,
                secret = TWITCH_SECRET
            };
            var stringPayload = JsonConvert.SerializeObject(payload);

            var logger = Context.GetLogger();
            logger.Log(Akka.Event.LogLevel.WarningLevel, $"Payload sending to Twitch: {stringPayload}");

            var responseMessage = await _Client.PostAsync(@"webhooks/hub", new StringContent(stringPayload, Encoding.UTF8, @"application/json"));
            if (!responseMessage.IsSuccessStatusCode) {
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                logger.Log(Akka.Event.LogLevel.ErrorLevel, $"Error response body: {responseBody}");
            }

        }

        public IActorRef ChatLogger { get; }

        public override void AroundPostStop()  {

            _Client.Dispose();

            base.AroundPostStop();

        }

        public class TwitchWebhookSubscriptionPayload {

            [JsonProperty("hub.callback")]
            public string callback { get; set; }

            [JsonProperty("hub.mode")]
            public string mode { get; set; }

            [JsonProperty("hub.topic")]
            public string topic { get; set; }

            [JsonProperty("hub.lease_seconds")]
            public int lease_seconds { get; set; }

            [JsonProperty("hub.secret")]
            public string secret { get; set; }

        }


    }

}