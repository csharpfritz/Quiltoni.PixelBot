using Quiltoni.PixelBot;
using Xunit.Abstractions;

namespace Quiltoni.Test
{
	class StubChat : IChatService
	{
		private readonly ITestOutputHelper _output;

		public StubChat(ITestOutputHelper output) {
			_output = output;
		}
		public void BroadcastMessageOnChannel(string message) {
			_output.WriteLine(message);
		}

		public void WhisperMessage(string username, string message) {
			_output.WriteLine($"{username}: {message}");
		}
	}
}
