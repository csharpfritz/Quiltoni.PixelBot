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

		/// <summary>
		/// Creates and adds a concrete implementation of a IResolveActor to the DI system.
		/// </summary>
		/// <typeparam name="Resolver">Concrete implementation of a IResolveActor</typeparam>
		/// <param name="services">Service collection of the DI</param>
		/// <param name="args">Additional parameters to pass on to the public static Create method of the IResolveActor</param>
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
	}
}
