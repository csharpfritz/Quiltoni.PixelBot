using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class ReportNewSubscriberForChannel {

		public ReportNewSubscriberForChannel(string channelName, string channelId, string userName, string userId, string message, short numMonths)
		{
			ChannelName = channelName;
			ChannelId = channelId;
			UserName = userName;
			UserId = userId;
			Message = message;
			NumberOfMonths = numMonths;
		}

		public string ChannelName { get; }
		public string ChannelId { get; }
		public string UserName { get; }
		public string UserId { get; }
		public short NumberOfMonths { get; }
		public string Message { get; }
	}

}