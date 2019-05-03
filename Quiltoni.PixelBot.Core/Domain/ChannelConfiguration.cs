using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Domain
{

	public class ChannelConfiguration
	{

		public string ChannelName { get; set; }

		public bool GuessGameEnabled { get; set; } = false;

		public CurrencyConfiguration Currency { get; set; } = new CurrencyConfiguration();

		// TODO: Add other configuration options later

	}


}
