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
			new ChannelConfiguration { ChannelName="csharpfritz", GuessGameEnabled=true },
			new ChannelConfiguration { ChannelName="instafluff" },
			new ChannelConfiguration { ChannelName="elliface" },
			new ChannelConfiguration { ChannelName="banzaibaby" },
			new ChannelConfiguration { ChannelName="visualstudio" },
			new ChannelConfiguration { ChannelName="chiefcll" },
			new ChannelConfiguration { ChannelName="quiltoni", GuessGameEnabled=true,
				Currency =new CurrencyConfiguration {
					Enabled=true,
					MyCommand = "mypixels",
					Name = "Pixels"				
				}
			}
		};

		public ChannelConfiguration GetConfigurationForChannel(string channelName) 
		{

			return _Configs.FirstOrDefault(c => c.ChannelName == channelName) ??
				new ChannelConfiguration { ChannelName = channelName };

		}
	}

}
