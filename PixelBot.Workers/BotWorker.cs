using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PixelBot.Workers
{
	public class BotWorker : BackgroundService
	{

		protected override Task ExecuteAsync(CancellationToken stoppingToken) {
			throw new NotImplementedException();
		}

	}
}
