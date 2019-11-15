using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Core.Data
{

	public interface IWidgetStateRepository
	{

		Task<Dictionary<string, string>> Get(string channelName, string widgetName);

		Task Save(string channelName, string widgetName, Dictionary<string, string> payload);

	}

}