using System;
using System.Collections.Generic;

namespace Quiltoni.PixelBot.Core.Data
{

	public interface ICurrencyRepository
	{

		IChatService ChatService { get; set; }

		void AddForChatters(string channel, int toAdd, string actingUser);

		void AddForUser(string userName, int toAdd, string actingUser);

		int FindForUser(string userName);

		IList<IList<object>> GetValuesFromSheet(string sheetName);

	}

}
