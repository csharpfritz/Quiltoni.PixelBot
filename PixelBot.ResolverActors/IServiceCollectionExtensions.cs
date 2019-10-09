using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace PixelBot.ResolverActors
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Add the ResolveActors to the root ActorSystem under akka://user/
		/// </summary>
		/// <param name="services">DI ServiceCollection</param>
		public static void AddResolveActors(this IServiceCollection services)
		{
			services.AddSingleton<ResolveActors>(provider => new ResolveActors(provider, provider.GetService<ActorSystem>()));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="services">DI ServiceCollection</param>
		/// <param name="actorRefFactory">The parent Actor that supervises these created IActorRefs. Any type in Akka.net that can supervise a child actor has IActorRefFactory.ActorOf() implemented.</param>
		public static void AddResolveActors(this IServiceCollection services, IActorRefFactory actorRefFactory)
		{
			services.AddSingleton<ResolveActors>(provider => new ResolveActors(provider, actorRefFactory));
		}
	}
}
