using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using PixelBot.Orchestrator.Actors;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using PixelBot.Orchestrator.Services.Authentication;
using PixelBot.ResolverActors;
using PixelBot.ResolverActors.Actors;
using Quiltoni.PixelBot.Core.Data;
using System.Security.Claims;

namespace PixelBot.Orchestrator
{
	public static class ConfigureServices
	{

		public static IServiceCollection ConfigureAspNet(this IServiceCollection services)
		{

			IdentityModelEventSource.ShowPII = true;

			#region Security

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
			.AddAuth0OpenIdConnect(Startup.Configuration);

			services.AddAuthorization(config =>
			{

				config.AddPolicy(nameof(Policy.GlobalAdmin), p =>
				{
					p.RequireRole("GlobalAdmin");
				});
			});

			#endregion

			services.AddMvc()
					.AddNewtonsoftJson();

			services.AddHealthChecks()
				.AddCheck<ActorSystemHealthCheck>("Actor System Health Check");

			services.AddHttpContextAccessor();
			services.AddScoped<ClaimsPrincipal>(context => context.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User);

			services.AddControllers();

			services.AddServerSideBlazor();
			services.AddSignalR(config =>
			{
				config.EnableDetailedErrors = true;
			}).AddJsonProtocol();

			return services;

		}

		public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
		{

			switch (Startup.Configuration["WidgetPersistence:Provider"].ToLowerInvariant())
			{
				case "azuretable":
					services.AddTransient<IWidgetStateRepository, AzureWidgetStateRepository>();
					break;
				case "json":
					services.AddTransient<IWidgetStateRepository, JsonWidgetStateRepository>();
					break;
				default:
					throw new ConfigurationException("Missing a WidgetPersistence Provider");
			}

			services.AddTransient<IChannelConfigurationContext, FileStorageChannelConfigurationContext>();

			services.AddTransient<IFollowerDedupeService, InMemoryFollowerDedupeService>();

			return services;

		}

		public static IServiceCollection ConfigureActorModel(this IServiceCollection services)
		{

			services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("BotService"));

			// Adds a ResolveServiceActor to the DI system
			services.AddResolveActor<ResolveServicesActor>();

			services.AddSingleton<IActorRef>(provider
				=> ChannelManagerActor.Create(provider.GetService<ActorSystem>())
				);

			return services;

		}

	}
}
