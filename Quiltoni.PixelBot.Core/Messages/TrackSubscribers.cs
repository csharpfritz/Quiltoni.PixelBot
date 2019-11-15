﻿using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class TrackSubscribers {

		public TrackSubscribers(string channelName, string channelId)
		{
			ChannelName = channelName;
			ChannelId = channelId;
		}

		public string ChannelName { get; }
		public string ChannelId { get; }

	}

}