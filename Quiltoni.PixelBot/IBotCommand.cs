using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot
{
	public interface IBotCommand 
	{

		string CommandText { get; }

		void Execute(ChatCommand command, IChatService twitch);

	}


}
