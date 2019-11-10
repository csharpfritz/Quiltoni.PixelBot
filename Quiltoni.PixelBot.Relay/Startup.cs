using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quiltoni.PixelBot.Relay.Controllers;
using Quiltoni.PixelBot.Relay.Models;

namespace Quiltoni.PixelBot.Relay
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddSignalR();

			services.Configure<StoreConfig>(options => Configuration.GetSection("Shops").Bind(options));
			//services.Configure<StoreConfig>(options => Configuration.GetSection("Shops").Bind(options));

			services.AddCors(options =>
						{
							options.AddPolicy("allowAll", builder =>
							{
								builder.AllowAnyMethod()
									.AllowAnyHeader()
									.WithOrigins("https://localhost:5001", "http://localhost:5000")
									.AllowCredentials();
							});
						});

			services.AddMvc(options =>
			{
				options.EnableEndpointRouting = false;
			}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// app.UseCors("allowAll");
			app.UseStaticFiles();

			app.UseSignalR(routes =>
			{
				routes.MapHub<NotificationHub>("/notifications");
			});

			if (!env.IsDevelopment())
			{
				app.UseHttpsRedirection();
			}
			app.UseMvc();
		}
	}
}