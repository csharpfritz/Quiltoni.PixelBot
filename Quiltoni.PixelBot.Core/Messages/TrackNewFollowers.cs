using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class TrackNewFollowers
	{

		public TrackNewFollowers(string channelName) {
			ChannelName = channelName;
		}

		public string ChannelName { get; }
	}

}
