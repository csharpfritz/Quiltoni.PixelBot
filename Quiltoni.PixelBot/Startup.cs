using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quiltoni.PixelBot.Commands;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Quiltoni.PixelBot
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration config)
		{

			this.Configuration = config;

		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddOptions();

			services.Configure<PixelBotConfig>(Configuration.GetSection("PixelBot"));

			// Register bot commands
			GetType().Assembly.GetTypes()
				.Where(t => t != typeof(IBotCommand) && typeof(IBotCommand).IsAssignableFrom(t))
				.ToList().ForEach(t => services.AddTransient(typeof(IBotCommand), t));

			_ = services.AddHttpClient("giveaway");

			_ = services.AddSingleton<IHostedService, PixelBot>()
				.AddSingleton<GuessGame>()
				.AddSingleton<GiveawayGame.GiveawayGame>();


			_ = services.AddMvc();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseMvc();

		}
	}
}
