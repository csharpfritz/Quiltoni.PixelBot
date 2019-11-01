using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Quiltoni.PixelBot.Relay
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();

			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder(args)
						.UseApplicationInsights()
						.UseStartup<Startup>();
	}
}