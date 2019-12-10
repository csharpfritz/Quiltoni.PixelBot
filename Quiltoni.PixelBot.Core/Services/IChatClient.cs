using System;
using System.Collections.Generic;
using System.Text;
using Quiltoni.PixelBot.Core.Messages;

namespace Quiltoni.PixelBot.Core.Services
{
	public interface IChatClient
	{

		// Cheer 500 lannonbr 10/12/19 

		/// <summary>
		/// Initialize the chat client so that it has the appropriate credentials to connect
		/// </summary>
		/// <param name="loginName"></param>
		/// <param name="password"></param>
		void Initialize(string loginName, string password);

		/// <summary>
		/// Raised when the chat client connects to the service
		/// </summary>
		event EventHandler OnConnected;

		event EventHandler<NewSubscriberArgs> OnNewSubscriber;

		event EventHandler<ResubscriberArgs> OnReSubscriber;

		event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

		event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

		event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

		event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

		void Connect();

		void Disconnect();

		void BroadcastMessage(string channelName, BroadcastMessage msg);

		void WhisperMessage(WhisperMessage msg);

	}
}
