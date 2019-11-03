using System;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Core
{
	public interface IOrderNotificationClient
	{

		Task OrderReceived(string orderId);

		Task RunRaffle(int winnerPosition, string[] entrants);
		Task Reset(bool v);
		Task AddEntrant(string newEntrant);
		Task AddEntrants(string[] newEntrants);
	}

}