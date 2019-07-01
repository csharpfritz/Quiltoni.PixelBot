using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Messages
{

	[Serializable]
	public sealed class WhisperMessage
	{

		public WhisperMessage(string userToWhisper, string message) {

			this.UserToWhisper = userToWhisper;
			this.Message = message;

		}

		public string UserToWhisper { get; }

		public string Message { get; }
	}

}
