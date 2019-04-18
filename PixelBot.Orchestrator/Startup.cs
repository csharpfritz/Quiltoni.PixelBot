using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PixelBot.Orchestrator.Components;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;

namespace PixelBot.Orchestrator
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc()
					.AddNewtonsoftJson();

			services.AddRazorComponents();

			services.AddSingleton<WeatherForecastService>();
			services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("BotService"));

			services.AddTransient<IChannelConfigurationContext, ChannelConfigurationContext>();


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

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting(routes => {
				routes.MapRazorPages();
				routes.MapComponentHub<App>("app");
			});
		}
	}
}
