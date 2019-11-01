using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core
{
	public interface IChatService
	{

		void BroadcastMessageOnChannel(string message);

		void WhisperMessage(string userDisplayName, string message);

	}
}