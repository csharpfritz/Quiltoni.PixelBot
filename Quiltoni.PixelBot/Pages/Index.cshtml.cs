using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Quiltoni.PixelBot.Pages
{
	public class IndexModel : PageModel
	{

		public IndexModel(IOptionsSnapshot<PixelBotConfig> config)
		{
			this.Config = config.Value;
			PixelBot.Config = config.Value;
		}

		public PixelBotConfig Config { get; }

		[BindProperty]
		[Required]
		public string BotName { get; set; }

		[BindProperty]
		[Required]
		public string Channel { get; set; }

		[BindProperty]
		[Required]
		public string TwitchAccessToken { get; set; }

		[BindProperty]
		public string SheetId { get; set; }

		public void OnGet() { }

		public async Task<IActionResult> OnPost() {

			if (ModelState.IsValid) {

				var jsonFile = JObject.Parse(System.IO.File.ReadAllText("appsettings.json"));
				var myRoot = jsonFile["PixelBot"];

				var twitchToken = myRoot["Twitch"];
				twitchToken["UserName"] = this.BotName;
				twitchToken["Channel"] = this.Channel;
				twitchToken["AccessToken"] = this.TwitchAccessToken;

				var googleToken = myRoot["Google"];
				googleToken["SheetId"] = SheetId;

				// Update the file
				await System.IO.File.WriteAllTextAsync("appsettings.json", jsonFile.ToString());
				await Task.Delay(1000);

				return RedirectToPage("./Index");

			}

			return Page();

		}

	}
}