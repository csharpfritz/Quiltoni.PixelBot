using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public sealed class LeaveChannel : IMessage
	{

		// cheer 1100 DeeDeeWalsh 22/10/2019
		// cheer 110 CopperBeardy 22/10/2019
		// cheer 550 faniereynders 22/10/2019

		public LeaveChannel(string channelName)
		{
			ChannelName = channelName;
		}

		public string ChannelName { get; }

	}

}