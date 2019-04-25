using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class BroadcastMessage
	{

		public BroadcastMessage(string message) {

			this.Message = message;

		}

		public string Message { get; }
	}
}
