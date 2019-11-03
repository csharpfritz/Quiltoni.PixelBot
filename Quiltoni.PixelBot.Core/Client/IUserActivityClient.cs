using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Core.Client
{

	public interface IUserActivityClient
	{

		Task NewFollower(string newFollowerName);

		Task NewSubscriber(string newSubscribedName, int numberOfMonths, int numberOfMonthsInRow, string message);

		Task NewCheer(string cheererName, int amountCheered, string message);

	}

}