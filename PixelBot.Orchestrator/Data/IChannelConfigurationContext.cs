using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Data
{
	public interface IChannelConfigurationContext
	{

		ChannelConfiguration GetConfigurationForChannel(string channelName);

		void SaveConfigurationForChannel(string channelName, ChannelConfiguration config);

	}

}
