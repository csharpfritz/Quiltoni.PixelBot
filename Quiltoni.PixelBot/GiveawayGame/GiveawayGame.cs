using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stateless;

namespace Quiltoni.PixelBot.GiveawayGame
{

	public class GiveawayGame
	{

		private readonly StateMachine<GiveawayGameState, GiveawayGameTrigger> _machine;

		private readonly List<string> _entrants = new List<string>();

		public GiveawayGame() {

			_machine = new StateMachine<GiveawayGameState, GiveawayGameTrigger>(GiveawayGameState.NotStarted);

		}

	}

}
