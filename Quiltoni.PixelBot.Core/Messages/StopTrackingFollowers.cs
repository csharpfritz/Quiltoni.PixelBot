using System;

namespace Quiltoni.PixelBot.Core.Messages
{
    [Serializable]
	public sealed class StopTrackingFollowers {

		public StopTrackingFollowers(string channelName, string channelId)
		{
            ChannelName = channelName;
            ChannelId = channelId;
        }

        public string ChannelName { get; }
        public string ChannelId { get; }
    }

}
