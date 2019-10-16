using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Messages
{

	[Serializable]
	public class GetConfigurationForChannel
	{

		public GetConfigurationForChannel(string channelName)
		{
			ChannelName = channelName;
		}

		public string ChannelName { get; }
	}

}
