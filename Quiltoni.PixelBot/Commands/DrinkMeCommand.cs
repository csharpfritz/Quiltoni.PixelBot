using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client.Models;

namespace Quiltoni.PixelBot.Commands
{
	public class DrinkMeCommand : IBotCommand, IRequiresSheet
	{

		static bool _First = true;
		private static IList<IList<object>> _Teas;

		public DrinkMeCommand(IConfiguration config) {

			_Configuration = config;

		}

		private readonly IConfiguration _Configuration;

		public bool Enabled => bool.Parse(_Configuration["PixelBot:Commands:DrinkMeCommand"]);

		public string CommandText => "drinkme";

		public ISheetProxy GoogleSheet { get; set; }

		public void Execute(ChatCommand command, IChatService twitch) {

			if (_First) {
				_First = false;
				_Teas = GoogleSheet.GetValuesFromSheet("DrinkMeTeas");
			}

			var theseTeas = new IList<object>[_Teas.Count];
			_Teas.CopyTo(theseTeas, 0);
			theseTeas = ShuffleTeas(theseTeas.ToList(), 5).ToArray();
			var randomPick = new Random().Next(0, theseTeas.Count());

			var theRecord = theseTeas.Skip(randomPick).First();
			twitch.BroadcastMessageOnChannel($"{command.ChatMessage.DisplayName} - you should drink {theRecord[0].ToString()} and you can find more at {theRecord[1].ToString()}");

		}

		private IList<IList<object>> ShuffleTeas(IList<IList<object>> teas, int shuffleCount) {

			var rdm = new Random();
			var outTeas = new List<IList<object>>();
			while (teas.Count > 0) {
				var thisTea = teas.Skip(rdm.Next(teas.Count)).First();
				outTeas.Add(thisTea);
				teas.Remove(thisTea);
			}

			shuffleCount--;
			if (shuffleCount == 0)
				return outTeas;

			return ShuffleTeas(outTeas, shuffleCount);

		}
	}

	public class Tea
	{
		
		public string Name { get; set; }

		public string Link { get; set; }

	}

}
