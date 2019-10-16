using Akka.Actor;
using PixelBot.ResolverActors.Messages;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PixelBot.ResolverActors.Actors
{
	/// <summary>
	/// Baseclas of an ReceiveActor to facilitate some basic common Resolver Actions
	/// </summary>
	/// <typeparam name="Message">The IResolveMessage<type>.</typeparam>
	/// <typeparam name="Container">The container to get something out from.</typeparam>
	/// <typeparam name="ActorBase">The ActorBase that is the parent actor of this actor</typeparam>
	public abstract class ResolveActor<Message, Container, ActorBase> : ReceiveActor
		where Message : IResolveMessage<Type>
		where ActorBase : Akka.Actor.ActorBase
	{

		protected Container _ResolverContainer;
		private static IActorRef _Instance;

		/// <summary>
		/// Delegate that resolves one object from the Container
		/// </summary>
		public abstract Func<Container, Message, object> ResolveOne { get; }

		/// <summary>
		/// Delegate that resolves IEnumerable<object> from the container
		/// </summary>
		public abstract Func<Container, Message, IEnumerable<object>> ResolveMany { get; }

		/// <summary>
		/// Delegate to replaces the container if needed.
		/// </summary>
		public abstract Func<Container, Container> ReplaceContainer { get; }

		protected ResolveActor(Container resolverContainer)
		{
			_ResolverContainer = resolverContainer;
			Receive<InitReslolveActor>(msg =>
			{
				Become(BecomeActive);
			});
		}
		private void BecomeActive()
		{
			Receive<Message>(msg => Resolve(msg));
			Receive<ReplaceContainer<Container>>(msg => _ResolverContainer = ReplaceContainer(msg.ReplacementContainer));
		}

		//BecomeActive();

		private void Resolve(Message msg)
		{
			if (msg.ResolveMany)
			{
				Sender.Tell(ResolveMany(_ResolverContainer, msg), Self);
			}
			else
			{
				Sender.Tell(ResolveOne(_ResolverContainer, msg), Self);
			}
		}

		public static IActorRef Instance
		{
			get
			{
				while (_Instance == null)
				{
					Thread.Sleep(1);
				}
				return _Instance;
			}

			private set => _Instance = value;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="resolverContainer"></param>
		/// <param name="actorContext"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected static IActorRef Create(Container resolverContainer, IActorRefFactory actorContext, string name = null, params object[] args)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = typeof(ActorBase).Name;
			}
			//var arguments = new object[] { resolverContainer };
			//if (args.Length > 0)
			//{
			//	int argLength = args.Length + 1;
			//	arguments = new object[argLength];
			//	arguments[0] = resolverContainer;
			//	args.CopyTo(arguments, 1);
			//}
			//var props = Props.Create<ActorBase>(arguments);
			var props = Props.Create<ActorBase>((Container)resolverContainer);
			Instance = actorContext.ActorOf(props, name);
			return Instance;
		}

	}
}
