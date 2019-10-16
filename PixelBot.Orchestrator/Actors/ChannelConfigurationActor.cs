using Akka.Actor;
using Newtonsoft.Json.Linq;
using PixelBot.Orchestrator.Data;
using PixelBot.ResolverActors;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Net.Http;

namespace PixelBot.Orchestrator.Actors
{
	public class ChannelConfigurationActor : ReceiveActor
	{
		private readonly IChannelConfigurationContext _Context;

		public IHttpClientFactory _ClientFactory { get; }

		public static string InstancePath
		{
			get { return BotConfiguration.ChannelConfigurationInstancePath; }
			private set { BotConfiguration.ChannelConfigurationInstancePath = value; }
		}

		public ChannelConfigurationActor()
		{
			_Context = this.RequestService<IChannelConfigurationContext>();

			_ClientFactory = this.RequestService<IHttpClientFactory>();

			InstancePath = Context.Self.Path.ToStringWithAddress();

			Receive<GetConfigurationForChannel>(this.GetConfigurationForChannel);
			Receive<SaveConfigurationForChannel>(this.SaveConfigurationForChannel);

		}

		private void SaveConfigurationForChannel(SaveConfigurationForChannel msg)
		{

			_Context.SaveConfigurationForChannel(msg.ChannelName, msg.Config);
			ChannelManagerActor.Instance.Tell(new NotifyChannelOfConfigurationUpdate(msg.ChannelName, msg.Config));

		}

		private void GetConfigurationForChannel(GetConfigurationForChannel msg)
		{

			var config = _Context.GetConfigurationForChannel(msg.ChannelName);

			if (String.IsNullOrEmpty(config.ChannelId))
			{

				config.ChannelId = GetChannelIdForChannel(msg.ChannelName);
				_Context.SaveConfigurationForChannel(msg.ChannelName, config);

			}

			Context.Sender.Tell(config);

		}

		private string GetChannelIdForChannel(string channelName)
		{

			using (var client = _ClientFactory.CreateClient("TwitchHelixApi"))
			{

				var msg = client.GetAsync($"users?login={channelName}");
				var body = msg.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				var obj = JObject.Parse(body);

				return obj["data"][0]["id"].ToString();

			}


		}
	}
}
