using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PixelBot.Orchestrator.Actors;
using PixelBot.Orchestrator.Components;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public static BotConfiguration BotConfiguration { get; private set; }

		public Startup(IConfiguration config) {

			this.Configuration = config;
		}

			// This method gets called by the runtime. Use this method to add services to the container.
			// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
			public void ConfigureServices(IServiceCollection services) {

			services.AddMvc()
					.AddNewtonsoftJson();

			services.AddControllers();

			services.AddServerSideBlazor();

			services.Configure<BotConfiguration>(Configuration.GetSection("BotConfig"));

			services.AddSingleton<WeatherForecastService>();
			services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("BotService"));

			services.AddTransient<IChannelConfigurationContext, ChannelConfigurationContext>();

			var provider = services.BuildServiceProvider();
			BotConfiguration = provider.GetService<IOptions<BotConfiguration>>().Value;

			// Cheer 100 ramblinggeek 19/4/19 

			var system = provider.GetService<ActorSystem>();
			var props = Props.Create<ChannelManagerActor>(provider.GetService<IChannelConfigurationContext>());
			services.AddSingleton<IActorRef>(_ => system.ActorOf(props));


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			else {
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseRouting();

			app.UseHttpsRedirection();
			app.UseStaticFiles();


			app.UseEndpoints(routes => {
				routes.MapRazorPages();
				routes.MapControllers();
				routes.MapBlazorHub<App>("app");
				routes.MapFallbackToPage("/Index");
			});
		}
	}
}
