using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PixelBot.Orchestrator
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
				Host.CreateDefaultBuilder(args)
						.ConfigureWebHostDefaults(webBuilder =>
						{
							webBuilder
								.UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
								.UseStaticWebAssets()
								.UseStartup<Startup>();
						});
	}
}