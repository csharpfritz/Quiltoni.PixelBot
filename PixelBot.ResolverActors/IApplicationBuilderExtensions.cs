using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PixelBot.ResolverActors.Actors;
using PixelBot.ResolverActors.Messages;
using System.Reflection;

namespace PixelBot.ResolverActors
{
	public static class IApplicationBuilderExtensions
	{
		public static void UseResolveActors(this IApplicationBuilder app)
		{
			var resolvers = app.ApplicationServices.GetServices<IResolveActor>();

			foreach (var resolver in resolvers)
			{
				var instance = resolver.GetType().BaseType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				var iActorRef = (IActorRef)instance.GetGetMethod().Invoke(null, null);
				iActorRef.Tell(new InitReslolveActor());
			}
		}
	}
}
