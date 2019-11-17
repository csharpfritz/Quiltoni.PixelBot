using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PixelBot.Orchestrator.Areas.Identity.Data;
using PixelBot.Orchestrator.Data;

namespace PixelBot.Orchestrator.Services.Authentication
{
	public static class AuthExtensions
	{
		public static AuthenticationBuilder AddAuth0OpenIdConnect(this AuthenticationBuilder builder, IConfiguration config)
		{

			if (Startup.EnvironmentName != "Production" && bool.Parse(config["MockTwitch"]) )
			{
				Startup.MockEnabled = true;
				return builder; 
			}

			Startup.MockEnabled = false;

			var domain = $"https://{config["Auth0:Domain"]}";
			var clientId = config["Auth0:ClientId"];
			var clientSecret = config["Auth0:ClientSecret"];

			return builder.AddOpenIdConnect("Auth0", options =>
			{
				// Set the authority to your Auth0 domain
				options.Authority = domain;

				// Configure the Auth0 Client ID and Client Secret
				options.ClientId = clientId;
				options.ClientSecret = clientSecret;

				// Set response type to code
				options.ResponseType = "code";

				// Configure the scope
				options.Scope.Clear();
				options.Scope.Add("openid");

				//for adding profile
				options.Scope.Add("profile");
				options.Scope.Add("email");
				options.Scope.Add("roles");


				//for adding profile
				options.TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name",
					RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/roles"
				};

				//for adding profile
				options.GetClaimsFromUserInfoEndpoint = true;

				// Set the callback path, so Auth0 will call back to http://localhost:3000/callback
				// Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
				options.CallbackPath = new PathString("/callback");

				// Configure the Claims Issuer to be Auth0
				options.ClaimsIssuer = "Auth0";

				//for adding profile
				options.SaveTokens = true;

				// Cheer 100 nothing_else_matters 4/10/19

				options.Events = new OpenIdConnectEvents
				{
					// handle the logout redirection
					OnRedirectToIdentityProviderForSignOut = (context) =>
					{
						var logoutUri = $"https://{config["Auth0:Domain"]}/v2/logout?client_id={config["Auth0:ClientId"]}";
						var postLogoutUri = context.Properties.RedirectUri;

						if (!string.IsNullOrEmpty(postLogoutUri))
						{
							if (postLogoutUri.StartsWith("/"))
							{
								// transform to absolute
								var request = context.Request;
								postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
							}
							logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
						}

						context.Response.Redirect(logoutUri);
						context.HandleResponse();

						return Task.CompletedTask;
					}
				};

			});
		}

		public static void AddLocalIdentity(this IServiceCollection services, IConfiguration config)
		{

			if (Startup.EnvironmentName == "Production" || !bool.Parse(config["MockTwitch"]))
			{
				return;
			}

			_ = services.AddDbContext<DbContext>(options => options.UseSqlite(SecurityDbContextFactory.SqliteConnectionString));

			services.AddDefaultIdentity<BotUser>()
				.AddEntityFrameworkStores<DbContext>();


			return;

		}  

	}

}