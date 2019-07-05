using System;
using System.Collections.Generic;
using System.Text;
using Quiltoni.PixelBot.Core.Extensibility;

namespace Quiltoni.PixelBot.Core.Domain
{

	public class ChannelConfiguration
	{

		public string ChannelName { get; set; }

		public bool GuessGameEnabled { get; set; } = false;

		public CurrencyConfiguration Currency { get; set; } = new CurrencyConfiguration();

		public BaseFeatureConfiguration GetFeatureConfiguration(string featureName) {
			return new BaseFeatureConfiguration();
		}

		// TODO: Add other configuration options later

	}


}
