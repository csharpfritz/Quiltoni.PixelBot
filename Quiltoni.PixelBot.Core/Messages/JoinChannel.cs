using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public sealed class JoinChannel : IMessage
	{

		public JoinChannel(string channelName)
		{

			this.ChannelName = channelName;

		}

		public string ChannelName { get; }


	}


	public interface IMessage { }

}