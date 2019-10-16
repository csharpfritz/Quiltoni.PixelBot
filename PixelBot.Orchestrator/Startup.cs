using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using PixelBot.Orchestrator.Actors;
using PixelBot.Orchestrator.Components;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using PixelBot.Orchestrator.Services.Authentication;
using PixelBot.ResolverActors;
using PixelBot.ResolverActors.Actors;
using PixelBot.StandardFeatures.ScreenWidgets.ChatRoom;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Data;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator
{
	public class Startup
	{
		public static IConfiguration Configuration { get; private set; }

		public static BotConfiguration BotConfiguration { get; private set; }

		public Startup(IConfiguration config)
		{

			Configuration = config;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			IdentityModelEventSource.ShowPII = true;


			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddAuth0OpenIdConnect(Configuration);

			services.AddAuthorization(config =>
			{

				config.AddPolicy(nameof(Policy.GlobalAdmin), p =>
				{
					p.RequireRole("GlobalAdmin");
				});
			});


			services.AddMvc()
					.AddNewtonsoftJson();

			services.AddHttpContextAccessor();
			services.AddScoped<ClaimsPrincipal>(context => context.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User);

			services.AddControllers();

			services.AddServerSideBlazor();
			services.AddSignalR(config =>
			{
				config.EnableDetailedErrors = true;
			}).AddJsonProtocol();

			services.Configure<BotConfiguration>(Configuration.GetSection("BotConfig"));

			services.ConfigureApplicationServices();

			// Adds a ResolveServiceActor to the DI system
			services.AddResolveActor<ResolveServicesActor>();

			services.AddTransient<IChannelConfigurationContext, FileStorageChannelConfigurationContext>();
			services.AddTransient<IFollowerDedupeService, InMemoryFollowerDedupeService>();

			services.AddHttpClient("TwitchHelixApi", config =>
			{

				config.BaseAddress = new Uri(Configuration["TwitchAPI:EndpointURL"]);
				config.DefaultRequestHeaders.Add("Accept", @"application/json");
				config.DefaultRequestHeaders.Add("Authorization", $"Bearer {Configuration["BotConfig:Password"]}");

			});

			// Cheer 100 ramblinggeek 19/4/19

			services.ConfigureActorModel();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<BotConfiguration> botConfig, ILoggerFactory loggerFactory)
		{

			// cheer 142 cpayette 4/10/2019
			// cheer 300 tbdgamer 4/10/2019

			var logger = loggerFactory.CreateLogger("Startup");
			logger.LogDebug($"Our Auth0 Domain is:  {Configuration["Auth0:Domain"]}");

			BotConfiguration = botConfig.Value;

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseRouting();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseAuthorization();

			// Initializes all ResolveActors
			app.UseResolveActors();

			// Configure plugged in features
			PluginBootstrapper.ServiceProvider = app.ApplicationServices;
			PluginBootstrapper.InitializeFeatures(app);

			app.UseEndpoints(routes =>
			{

				routes.MapHub<LoggerHub>("/loggerhub");
				routes.MapHub<UserActivityHub>("/useractivityhub");
				MapExternalHubs(routes);
				routes.MapHealthChecks("/health");
				routes.MapRazorPages();
				routes.MapDefaultControllerRoute();
				routes.MapBlazorHub();
				routes.MapFallbackToPage("/Index");
			});
		}

		private static void MapExternalHubs(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder routes)
		{

			// TODO: use reflection to identify Hubs in the StandardFeatures assembly and add them
			routes.MapHub<ChatRoomHub>("/hubs/chatroom");

		}
	}
}
