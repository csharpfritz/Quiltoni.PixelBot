using Microsoft.AspNetCore.SignalR;
using Quiltoni.PixelBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Relay.Controllers
{
	public class NotificationHub : Hub<IOrderNotificationClient>
	{

		public static int ConnectedCount = 0;

		public override Task OnConnectedAsync()
		{

			Interlocked.Increment(ref ConnectedCount);

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{

			Interlocked.Decrement(ref ConnectedCount);

			return base.OnDisconnectedAsync(exception);
		}

	}
}