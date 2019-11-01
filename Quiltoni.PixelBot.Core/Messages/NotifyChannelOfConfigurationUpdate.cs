using Quiltoni.PixelBot.Core.Domain;
using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class NotifyChannelOfConfigurationUpdate : SaveConfigurationForChannel
	{
		public NotifyChannelOfConfigurationUpdate(string channelName, ChannelConfiguration config) : base(channelName, config)
		{
		}
	}

}