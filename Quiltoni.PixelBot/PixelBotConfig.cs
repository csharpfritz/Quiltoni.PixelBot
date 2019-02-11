using System.Collections.Generic;

namespace Quiltoni.PixelBot
{

	public class PixelBotConfig
	{
		public TwitchConfig Twitch { get; set; }

		public GoogleConfig Google { get; set; }

		public CurrencyConfig Currency { get; set; }

		public Dictionary<string, bool> Commands { get; set; }

		public class TwitchConfig
		{
			public string UserName { get; set; }
			public string Channel { get; set; }
			public string AccessToken { get; set; }
		}

		public class GoogleConfig
		{

			public string ClientId { get; set; }

			public string ClientSecret { get; set; }

			public string SheetId { get; set; }

		}

		public class CurrencyConfig
		{

			public string Name { get; set; } = "pixels";

			public string MyCommand { get; set; } = "mypixels";

			public string SheetType { get; set; } = "GoogleSheetProxy";

		}

	}

}