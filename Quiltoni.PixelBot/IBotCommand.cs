using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot
{
	public interface IBotCommand 
	{

		bool Enabled { get; }

		string CommandText { get; }

		void Execute(ChatCommand command, IChatService twitch);

	}


}
