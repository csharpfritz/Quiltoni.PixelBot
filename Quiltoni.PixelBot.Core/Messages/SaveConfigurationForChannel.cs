using Quiltoni.PixelBot.Core.Domain;
using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class SaveConfigurationForChannel
	{

		public SaveConfigurationForChannel(string channelName, ChannelConfiguration config)
		{
			ChannelName = channelName;
			Config = config;
		}

		public string ChannelName { get; }

		public ChannelConfiguration Config { get; }
	}

}