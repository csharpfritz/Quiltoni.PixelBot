using System;

namespace Quiltoni.PixelBot.Core.Messages
{

	[Serializable]
	public class RejoinChannels { }

	[Serializable]
	public class GetChannelsToReconnect {

	}


	[Serializable]
	public class ChannelsToReconnect {

		public ChannelsToReconnect(string[] channels)
		{
			Channels = channels;
		}

		public string[] Channels { get; }

	}

}
