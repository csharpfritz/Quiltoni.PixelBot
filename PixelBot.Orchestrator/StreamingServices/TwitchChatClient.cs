using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quiltoni.PixelBot.Core.Messages;
using Quiltoni.PixelBot.Core.Services;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using WhisperMessage = Quiltoni.PixelBot.Core.Messages.WhisperMessage;

namespace PixelBot.Orchestrator.StreamingServices
{

	public class TwitchChatClient : IChatClient
	{
		private TwitchClient _Client;

		public event EventHandler OnConnected;
		public event EventHandler<NewSubscriberArgs> OnNewSubscriber;
		public event EventHandler<ResubscriberArgs> OnReSubscriber;
		public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;
		public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;
		public event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;
		public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

		public TwitchChatClient()
		{
			_Client = new TwitchClient();
		}


		public void BroadcastMessage(string channelName, BroadcastMessage msg)
		{

			_Client.SendMessage(channelName, msg.Message);

		}

		public void Connect()
		{

			_Client.OnNewSubscriber += (o, args) => OnNewSubscriber?.Invoke(this, new NewSubscriberArgs());
			_Client.OnReSubscriber += (o, args) => OnReSubscriber?.Invoke(this, new ResubscriberArgs());
			_Client.OnGiftedSubscription += (o, args) => OnGiftedSubscription?.Invoke(this, new OnGiftedSubscriptionArgs());
			_Client.OnRaidNotification += (o, args) => OnRaidNotification?.Invoke(this, new OnRaidNotificationArgs());
			_Client.OnChatCommandReceived += (o, args) => OnChatCommandReceived?.Invoke(this, new OnChatCommandReceivedArgs());
			_Client.OnMessageReceived += (o, args) => OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs());

			_Client.Connect();
		}

		public void Disconnect()
		{
			_Client.Disconnect();
		}

		public void Initialize(string loginName, string password)
		{

			var creds = new ConnectionCredentials(loginName, password);
			_Client.Initialize(creds);

		}

		public void WhisperMessage(WhisperMessage msg)
		{
			_Client.SendWhisper(msg.UserToWhisper, msg.Message);
		}

	}
}
