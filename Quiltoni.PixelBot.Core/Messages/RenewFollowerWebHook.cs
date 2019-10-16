using System;

namespace Quiltoni.PixelBot.Core.Messages
{
    [Serializable]
	public class RenewFollowerWebHook {

		public RenewFollowerWebHook(string channelId)
		{
			
			this.ChannelId = channelId;

		}

		public string ChannelId { get; }

	}

}
