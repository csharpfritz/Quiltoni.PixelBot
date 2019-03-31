using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quiltoni.PixelBot.Relay.Controllers;
using Xunit;

namespace Quiltoni.Test.GiveawayGame
{

	public class RandomWinner
	{

		[Theory]
		[InlineData(25)]
		[InlineData(50)]
		[InlineData(100)]
		[InlineData(500)]
		[InlineData(1000)]
		public void ShouldPickRandomWinners(int entrantCount) {

			var results = new Dictionary<int, int>();

			for (var i=0;i<entrantCount*2; i++) {

				var winner = GiveawayGameController.RandomWinner(entrantCount);
				results[winner] = results.ContainsKey(winner) ? results[winner] + 1 : 1;

			}

			if (entrantCount == 25) Assert.Contains(results, kv => kv.Key == 24);
			Assert.DoesNotContain(results, kv => kv.Value > Math.Ceiling(entrantCount*0.3M));

		}

	}

}
