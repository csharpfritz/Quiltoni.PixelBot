using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Services
{
	public enum Policy
	{

		GlobalAdmin

	}

}

namespace Microsoft.AspNetCore.Authorization
{

	public static class AuthorizationServiceExtensions
	{

		public static async Task<bool> AuthorizeAsync(this IAuthorizationService svc, System.Security.Claims.ClaimsPrincipal user, PixelBot.Orchestrator.Services.Policy policy)
		{

			var result = await svc.AuthorizeAsync(user, nameof(policy));
			return result.Succeeded;

		}

		public static bool Authorize(this IAuthorizationService svc, System.Security.Claims.ClaimsPrincipal user, PixelBot.Orchestrator.Services.Policy policy)
		{

			return svc.AuthorizeAsync(user, policy.ToString()).GetAwaiter().GetResult().Succeeded;

		}

	}

}