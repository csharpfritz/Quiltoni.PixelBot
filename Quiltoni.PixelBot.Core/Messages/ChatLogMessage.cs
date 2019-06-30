using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public sealed class ChatLogMessage
	{

		public ChatLogMessage(LogLevel level, string channel, string message) {

			this.LogLevel = level;
			this.Channel = channel;
			this.Message = message;

		}

		public LogLevel LogLevel { get; }
		public string Channel { get; }
		public string Message { get; }
	}
}
