using Newtonsoft.Json;

namespace PixelBot.Orchestrator.Services.Authentication
{
	public class Auth0Authorization
	{
		[JsonProperty("groups")]
		public string[] Groups { get; set; }
	}

}
