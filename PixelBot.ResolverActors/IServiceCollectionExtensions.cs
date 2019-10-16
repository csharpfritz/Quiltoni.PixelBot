using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using PixelBot.ResolverActors.Actors;
using PixelBot.ResolverActors.Messages;
using System;
using System.Reflection;

namespace PixelBot.ResolverActors
{
	public static class IServiceCollectionExtensions
	{

		//public ResolveActors(IServiceProvider provider, IActorRefFactory actorRefFactory)
		//{
		//	ServicesActor = ResolveServicesActor.Create(provider, actorRefFactory);
		//	ServicesActor.Tell(new InitReslolveActor());
		//}


		public static void AddResolveActor<Resolver>(this IServiceCollection services, params object[] args)
			where Resolver : IResolveActor
		{
			services.AddSingleton<IResolveActor>(provider => AddResolver(provider, typeof(Resolver), args));
		}

		private static IResolveActor AddResolver(IServiceProvider provider, Type type, params object[] args)
		{
			var creater = type.GetMethod("Create", BindingFlags.Static)
				?? throw new ArgumentException($"{type.FullName} doesn't implements a public static Create method!");
			var instance = type.GetProperty("Instance", BindingFlags.Static) ?? throw new ArgumentException($"{type.FullName} doesn't implements a public static Instance property!");


			var actorRefFactory = provider.GetService<ActorSystem>();
			var resolver = (IActorRef)creater.Invoke(null, args.BuildArgs(provider, actorRefFactory));
			resolver.Tell(new InitReslolveActor());

			return (IResolveActor)instance.GetGetMethod().Invoke(null, null);
		}

		/// <summary>
		/// Add the ResolveActors to the root ActorSystem under akka://user/
		/// </summary>
		/// <param name="services">DI ServiceCollection</param>
		public static void AddResolveActors(this IServiceCollection services)
		{

		}

	}
}
