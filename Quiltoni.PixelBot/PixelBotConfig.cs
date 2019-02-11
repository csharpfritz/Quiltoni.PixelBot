namespace Quiltoni.PixelBot
{

	public class PixelBotConfig
	{
		public TwitchConfig Twitch { get; set; }
		public GoogleConfig Google { get; set; }

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

	}

}