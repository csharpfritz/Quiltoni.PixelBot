using System.Collections.Generic;

namespace Quiltoni.PixelBot
{
	public interface ISheetProxy
	{

		IChatService Twitch { get; set; }

		void AddPixelsForChatters(string channel, int pixelsToAdd, string actingUser);

		void AddPixelsForUser(string userName, int numPixelsToAdd, string actingUser);

		int FindPixelsForUser(string userName);

		IList<IList<object>> GetValuesFromSheet(string sheetName);

	}
}
