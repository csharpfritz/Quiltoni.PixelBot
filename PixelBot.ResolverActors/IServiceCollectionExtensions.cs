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

	}
}
