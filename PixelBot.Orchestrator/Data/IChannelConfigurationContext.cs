using System.Collections.Generic;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Data
{
	public interface IChannelConfigurationContext
	{

		ChannelConfiguration GetConfigurationForChannel(string channelName);

		IEnumerable<string> GetConnectedChannels();

		void SaveConfigurationForChannel(string channelName, ChannelConfiguration config);

	}

}