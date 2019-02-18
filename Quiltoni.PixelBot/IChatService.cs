namespace Quiltoni.PixelBot
{
	public interface IChatService {

		void BroadcastMessageOnChannel(string message);

		void WhisperMessage(string userDisplayName, string message);

	}

}
