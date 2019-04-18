using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Data
{


	public class ChannelConfigurationContext : IChannelConfigurationContext
	{

		private static readonly IList<ChannelConfiguration> _Configs = new List<ChannelConfiguration> {
			new ChannelConfiguration { ChannelName="csharpfritz" },
			new ChannelConfiguration { ChannelName="quiltoni" }
		};

		public ChannelConfiguration GetConfigurationForChannel(string channelName) {

			return _Configs.FirstOrDefault(c => c.ChannelName == channelName);

		}
	}

}
