using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using PixelBot.ResolverActors.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelBot.ResolverActors.Actors
{
	/// <summary>
	/// Specializes a ResolveActor to use the Message RequestService, the .net Core dependency container IServiceProvider
	/// </summary>
	public class ResolveServicesActor : ResolveActor<RequestService, IServiceProvider, ResolveServicesActor>
	{
		public const string Name = "ResolveServices";

		private ResolveServicesActor(IServiceProvider services) : base(services)
		{
		}

		/// <summary>
		/// Just forwards to ResolveMany and returns the first or the default value.
		/// </summary>
		public override Func<IServiceProvider, RequestService, object> ResolveOne
		=> (diContainer, msg) => ResolveMany(diContainer, msg).FirstOrDefault();

		/// <summary>
		/// Uses the extension method GetServices from Microsoft.Extensions.DependencyInjection to get all requested objects.
		/// </summary>
		public override Func<IServiceProvider, RequestService, IEnumerable<object>> ResolveMany
			=> (diContainer, msg) => diContainer.GetServices(msg.Requested);

		/// <summary>
		/// Indicates that this operation is on this DiContainer not allowed. Always throws a NotSupportedException!
		/// </summary>
		public override Func<IServiceProvider, IServiceProvider> ReplaceContainer
			=> throw new NotSupportedException();

		/// <summary>
		/// Forwards any IActorRef creation attempt to the baseclas.
		/// </summary>
		/// <param name="container">.net Core dependency container IServiceProvider</param>
		/// <param name="actorContext">The parent Actor that supervises this created IActorRef. Any type in Akka.net that can supervise a child actor has IActorRefFactory.ActorOf() implemented.</param>
		/// <returns></returns>
		public static IActorRef Create(IServiceProvider container, IActorRefFactory actorContext)
			=> (IActorRef)Create(container, actorContext, Name);
	}
}
