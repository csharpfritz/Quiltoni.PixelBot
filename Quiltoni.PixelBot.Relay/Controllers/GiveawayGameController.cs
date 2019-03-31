using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Quiltoni.PixelBot.Core;

namespace Quiltoni.PixelBot.Relay.Controllers
{

	// Cheer 100 cpayette 31/3/19 
	// Cheer 500 faniereynders 31/3/19 
	// Cheer 391 devlead 31/3/19 
	// Cheer 1000 animatedslinky 31/3/19 
	// Cheer 492 roberttables 31/3/19 
	// Cheer 1200 tealoldman 31/3/19 
	// Cheer 200 smallbacon 31/3/19 

	// Cheer 5000 phrakberg 04/4/19 
	// Cheer 1000 deedeewalsh 04/4/19 
	// Cheer 300 cpayette 04/4/19 
	// Cheer 1140 electrichavoc 04/4/19 
	// Cheer 2201 themichaeljolley 04/4/19 
	// Cheer 100 sorskoot 04/4/19 


	[ApiController]
	[Route("api/[controller]")]
	public class GiveawayGameController : ControllerBase
	{
		private IHubContext<NotificationHub, IOrderNotificationClient> _HubContext;

		public GiveawayGameController(IHubContext<NotificationHub, IOrderNotificationClient> hubContext) {
			_HubContext = hubContext;
		}

		/**
		 *	Note:  Need to indicate / confirm who has joined the raffle
				Note: No-repeats ---  maintain an array of winners and pass into the random winner selection
		*/

		[HttpPost()]
		public async Task<IActionResult> Post([FromBody]string[] entrants) {

			var theWinner = RandomWinner(entrants.Count());
			await _HubContext.Clients.All.RunRaffle(theWinner, entrants);
			return Ok(entrants[theWinner]);

		}

		[HttpPut]
		public async Task<IActionResult> Put([FromBody]string[] newEntrant) {

			// Cheer 1950 phrakberg 09/4/19 
			// Cheer 426 cpayette 11/4/19 
			// Cheer 200 electrichavoc 11/4/19 

			if (newEntrant.Count() > 1) {
				await _HubContext.Clients.All.AddEntrants(newEntrant);
			} else {
				await _HubContext.Clients.All.AddEntrant(newEntrant[0]);
			}
			return Ok();

		}

		[HttpGet]
		public async Task<IActionResult> Get() {

			await _HubContext.Clients.All.Reset(true);
			return Ok();

		}

		public static int RandomWinner(int count) {

			var list = Enumerable.Range(0, count).ToList();
			var rdm = new Random((int)DateTime.Now.TimeOfDay.Ticks);

			for (var i=0; i<10; i++) {
				list = Shuffle(list);
				Debug.WriteLine(string.Join(',', list.ToArray()));
			}

			return list.First();

			List<int> Shuffle(List<int> entries) {

				var outList = new List<int>(entries.Count());

				while(entries.Any()) { 
					var selected = entries.Skip(rdm.Next(entries.Count())).First();
					outList.Add(selected);
					entries.Remove(selected);
				}

				return outList;

			}

		}

	}

}
