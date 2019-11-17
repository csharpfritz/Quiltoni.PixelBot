using Akka.Actor;
using Akka.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using PixelBot.Orchestrator.Actors;
using PixelBot.Orchestrator.Data;
using PixelBot.Orchestrator.Services;
using PixelBot.Orchestrator.Services.Authentication;
using Quiltoni.PixelBot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

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

			services.AddLocalIdentity(Startup.Configuration);

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

			switch (Startup.Configuration["ChannelConfiguration:Provider"].ToLowerInvariant())
			{
				case "azuretable":
					services.AddTransient<IChannelConfigurationContext, AzureChannelConfigurationContext>();
					break;
				case "json":
					services.AddTransient<IChannelConfigurationContext, FileStorageChannelConfigurationContext>();
					break;
			}


			services.AddTransient<IChannelConfigurationContext, FileStorageChannelConfigurationContext>();

			services.AddTransient<IFollowerDedupeService, InMemoryFollowerDedupeService>();

			return services;

		}

		public static IServiceCollection ConfigureActorModel(this IServiceCollection services)
		{

			services.AddSingleton<ActorSystem>(_ => ActorSystem.Create("BotService"));

			services.AddSingleton<IActorRef>(provider => ChannelManagerActor.Create(
				provider.GetService<ActorSystem>(),
				provider
			));

			return services;

		}

	}
}