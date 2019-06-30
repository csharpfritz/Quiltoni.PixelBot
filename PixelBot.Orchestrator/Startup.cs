using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PixelBot.Orchestrator.Actors;
using PixelBot.Orchestrator.Components;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using PixelBot.Orchestrator.Services.Authentication;
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

			services.Configure<CookiePolicyOptions>(options => {
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddAuth0OpenIdConnect(Configuration);

			services.AddAuthorization(config => {

				config.AddPolicy(nameof(Policy.GlobalAdmin), p => {
					p.RequireRole("GlobalAdmin");
				});
			});

			services.AddMvc()
					.AddNewtonsoftJson();

			services.AddHttpContextAccessor();
			services.AddScoped<ClaimsPrincipal>(context => context.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User);

			services.AddControllers();

			services.AddServerSideBlazor();
			services.AddSignalR(config => {
				config.EnableDetailedErrors = true;
			}).AddJsonProtocol();


			services.Configure<BotConfiguration>(Configuration.GetSection("BotConfig"));

			services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("BotService"));

			services.AddTransient<IChannelConfigurationContext, ChannelConfigurationContext>();

			// Cheer 100 ramblinggeek 19/4/19 

			services.AddSingleton<IActorRef>(provider => ChannelManagerActor.Create(
				provider.GetService<ActorSystem>(),
				provider.GetService<IChannelConfigurationContext>(),
				provider.GetService<IHubContext<LoggerHub, IChatLogger>>()
			));

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<BotConfiguration> botConfig) {

			BotConfiguration = botConfig.Value;

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

			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(routes => {
				routes.MapHub<LoggerHub>("/loggerhub");
				routes.MapRazorPages();
				routes.MapDefaultControllerRoute();
				routes.MapBlazorHub();
				routes.MapFallbackToPage("/Index");
			});
		}
	}
}
