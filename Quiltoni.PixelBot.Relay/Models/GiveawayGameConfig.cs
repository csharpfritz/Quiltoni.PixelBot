using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Relay.Models
{
	public class GiveawayGameConfig
	{

		public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromSeconds(5);

		public bool NoRepeatWinners { get; set; } = false;

		public string BackgroundColor { get; set; } = "#000";

		public string FontColor { get; set; } = "#FFF";

		public string Font { get; set; } = "Sans Serif";

	}
}