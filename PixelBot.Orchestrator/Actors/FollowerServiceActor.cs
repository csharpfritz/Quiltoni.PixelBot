using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

	public class FollowerServiceActor : ReceiveActor
	{
		public const string TWITCH_SECRET = "MYSECRET";
		private readonly IHttpClientFactory _ClientFactory;
		private static readonly HashSet<string> _FollowerChannels = new HashSet<string>();

		public FollowerServiceActor(
				IHttpClientFactory httpClientFactory,
				IConfiguration configuration,
				IWebHostEnvironment env)
		{

			_Configuration = configuration;
			_Env = env;

			this.ChatLogger = Context.ActorSelection(ChatLoggerActor.Path)
				.ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

			this.ReceiveAsync<TrackNewFollowers>(AddChannelToTrack);
			this.ReceiveAsync<RenewFollowerWebHook>(m => SubscribeToTwitchWebhook(m.ChannelName, m.ChannelId));

			this.Receive<StopTrackingFollowers>(StopTracking);

			_ClientFactory = httpClientFactory;

		}

		private void StopTracking(StopTrackingFollowers arg)
		{

			if (!_FollowerChannels.Contains(arg.ChannelName)) return;

			_FollowerChannels.Remove(arg.ChannelName);

		}

		private async Task AddChannelToTrack(TrackNewFollowers arg)
		{

			// Check if the channel is already subscribed to with the WebHook API
			if (_FollowerChannels.Contains(arg.ChannelName)) return;

			_FollowerChannels.Add(arg.ChannelName);

			await SubscribeToTwitchWebhook(arg.ChannelName, arg.ChannelId);

			ChatLogger.Tell(new ChatLogMessage(Microsoft.Extensions.Logging.LogLevel.Debug, "- global -", $"Now tracking followers for channel {arg.ChannelName}"));

		}

		private async Task SubscribeToTwitchWebhook(string channelName, string channelId)
		{

			if (!_FollowerChannels.Contains(channelName)) return;

			var leaseInSeconds = _Env.IsDevelopment() ? 300 : 3600;

			var callbackUrl = _Env.IsDevelopment() ? await GetNgrokTunnelUri() : new Uri(_Configuration["HostUrl"]);


			var payload = new TwitchWebhookSubscriptionPayload
			{
				callback = new Uri(callbackUrl, "/api/follower").ToString(),
				mode = "subscribe",
				topic = $"https://api.twitch.tv/helix/users/follows?first=1&to_id={channelId}",
				lease_seconds = leaseInSeconds,
				secret = TWITCH_SECRET
			};
			var stringPayload = JsonConvert.SerializeObject(payload);

			var logger = Context.GetLogger();
			logger.Log(Akka.Event.LogLevel.WarningLevel, $"Payload sending to Twitch: {stringPayload}");

			using (var client = _ClientFactory.CreateClient("TwitchHelixApi"))
			{

				var responseMessage = await client.PostAsync(@"webhooks/hub", new StringContent(stringPayload, Encoding.UTF8, @"application/json"));
				if (!responseMessage.IsSuccessStatusCode)
				{
					var responseBody = await responseMessage.Content.ReadAsStringAsync();
					logger.Log(Akka.Event.LogLevel.ErrorLevel, $"Error response body: {responseBody}");
				}
				else
				{

					// Schedule a lease renewal
					logger.Log(Akka.Event.LogLevel.WarningLevel, $"Scheduling lease renewal for: {channelId}");
					Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(leaseInSeconds - 30),
							Self, new RenewFollowerWebHook(channelName, channelId), Self);

				}

			}

		}

		private readonly IConfiguration _Configuration;
		private readonly IWebHostEnvironment _Env;

		public IActorRef ChatLogger { get; }

		public override void AroundPostStop()
		{

			// TODO: Should we unsubscribe from our Webhook subscriptions?

			base.AroundPostStop();

		}

		private async Task<Uri> GetNgrokTunnelUri()
		{

			Uri outValue = null;

			using (var client = _ClientFactory.CreateClient())
			{

				client.DefaultRequestHeaders.Add("Accept", "application/json");
				var payload = await client.GetStringAsync("http://127.0.0.1:4040/api/tunnels");

				outValue = new Uri(JObject.Parse(payload)["tunnels"][0]["public_url"].ToString());

				var logger = Context.GetLogger();
				logger.Log(Akka.Event.LogLevel.DebugLevel, $"Fetched NGROK public url of : {outValue}");

			}

			return outValue;

		}


		public class TwitchWebhookSubscriptionPayload
		{

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