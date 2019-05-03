namespace Quiltoni.PixelBot.Core.Domain
{

	public class CurrencyConfiguration {

		public bool Enabled { get; set; } = false;

		public string Name { get; set; } = "Coins";

		public string MyCommand { get; set; } = "mycoins";

		public int AwardForSub_Prime { get; set; } = 10;

		public int AwardForSub_Tier1 { get; set; } = 10;

		public int AwardForSub_Tier2 { get; set; } = 25;

		public int AwardForSub_Tier3 { get; set; } = 60;

		public int AwardForGiftSub { get; set; } = 2;

		public int AwardForRaid_Min { get; set; } = 3;

		public int AwardForRaid_PerViewer { get; set; } = 1;

		public int AwardForRaid_Max { get; set; } = 200;

	}

}