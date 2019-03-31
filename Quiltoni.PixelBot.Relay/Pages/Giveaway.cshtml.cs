using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quiltoni.PixelBot.Relay.Pages
{
	public class GiveawayModel : PageModel
	{
		public void OnGet() {
			this.Config = new Models.GiveawayGameConfig();
		}

		public Models.GiveawayGameConfig Config { get; set; }

	}
}