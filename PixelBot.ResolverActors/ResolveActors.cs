using Akka.Actor;
using PixelBot.ResolverActors.Actors;
using System;

namespace PixelBot.ResolverActors
{
	/// <summary>
	/// A type that holds IActorRefs to Actors that resolves instances from some sort of container. This type exists to facilitate a better integration to dependency injection containers that make it difficult or complicated to resolve multiple instances of IActorRef.
	/// </summary>
	public class ResolveActors
	{
		/// <summary>
		/// IActorRef of the actor that resolves instances from a IServiceProvider
		/// </summary>
		public IActorRef ServicesActor { get; }

		/// <summary>
		/// Initiates and creates all used ResolveActors
		/// </summary>
		/// <param name="provider">The DI provider used by the system to facilitates resolving instances from a DI container.</param>
		/// <param name="actorRefFactory">The parent Actor that supervises these created IActorRefs. Any type in Akka.net that can supervise a child actor has IActorRefFactory.ActorOf() implemented.</param>
		public ResolveActors(IServiceProvider provider, IActorRefFactory actorRefFactory)
		{
			ServicesActor = ResolveServicesActor.Create(provider, actorRefFactory);
		}
	}
}
