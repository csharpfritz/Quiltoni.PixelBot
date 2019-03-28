using System;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Core
{
	public interface IOrderNotificationClient
	{

		Task OrderReceived(string orderId);

		Task RunRaffle(string[] entrants);

	}

}
