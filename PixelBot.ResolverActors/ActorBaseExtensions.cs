using Akka.Actor;
using PixelBot.ResolverActors.Actors;
using PixelBot.ResolverActors.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelBot.ResolverActors
{
	public static class ActorBaseExtensions
	{
		/// <summary>
		/// Adds a handy asynchronous extension method to ActorBase to being able to resolve Services inside an Actor more easier.
		/// </summary>
		/// <typeparam name="TService">The service type that needs to be resolved</typeparam>
		/// <param name="actor">this.ActorBase</param>
		/// <returns>TService instance if found otherwise default.</returns>
		public static async Task<TService> RequestServiceAsync<TService>(this ActorBase actor)
			=> await RequestServiceAsync<TService>(typeof(TService));

		/// <summary>
		/// Adds a handy extension method to ActorBase to being able to resolve Services inside an Actor more easier. (.GetAwaiter().GetResult()s the asynchronous method for getting the service.)
		/// </summary>
		/// <typeparam name="TService">The service type that needs to be resolved.</typeparam>
		/// <param name="actor">this.ActorBase</param>
		/// <returns>TService instance if found otherwise default.</returns>
		public static TService RequestService<TService>(this ActorBase actor)
			=> RequestServiceAsync<TService>(typeof(TService)).GetAwaiter().GetResult();

		/// <summary>
		/// Adds a handy extension method to ActorBase to being able to resolve many Services of one type at once inside an Actor more easier. (.GetAwaiter().GetResult()s the asynchronous method for getting the service.)
		/// </summary>
		/// <typeparam name="TService">The service type that needs to be resolved.</typeparam>
		/// <param name="actor">this.ActorBase</param>
		/// <returns>IEnumerable of TService instances if found otherwise default.</returns>
		public static IEnumerable<TService> RequestManyServices<TService>(this ActorBase actor)
	=> RequestManyServicesAsync<TService>(typeof(TService)).GetAwaiter().GetResult();

		/// <summary>
		/// Adds a handy asynchronous extension method to ActorBase to being able to resolve many Services of one type at once inside an Actor more easier.
		/// </summary>
		/// <typeparam name="TService">The service type that needs to be resolved.</typeparam>
		/// <param name="actor">this.ActorBase</param>
		/// <returns>IEnumerable of TService instances if found otherwise default.</returns>
		public static async Task<IEnumerable<TService>> RequestManyServicesAsync<TService>(this ActorBase actor)
			=> await RequestManyServicesAsync<TService>(typeof(TService));

		private static async Task<TService> RequestServiceAsync<TService>(Type type)
		{
			var requestMsg = new RequestService(type, false);
			return (TService)await ResolveServicesActor.Instance.Ask(requestMsg);
		}

		private static async Task<IEnumerable<TService>> RequestManyServicesAsync<TService>(Type type)
		{
			var requestMsg = new RequestService(type, true);
			return (IEnumerable<TService>)await ResolveServicesActor.Instance.Ask(requestMsg);
		}
	}
}
