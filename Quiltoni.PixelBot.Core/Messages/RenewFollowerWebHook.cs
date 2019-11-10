using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class RenewFollowerWebHook
	{

		public RenewFollowerWebHook(string channelName, string channelId)
		{
			ChannelName = channelName;
			this.ChannelId = channelId;

		}

		public string ChannelName { get; }
		public string ChannelId { get; }

	}

}