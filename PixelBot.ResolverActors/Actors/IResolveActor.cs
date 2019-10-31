using PixelBot.ResolverActors.Messages;
using System;
using System.Collections.Generic;

namespace PixelBot.ResolverActors.Actors
{
	public interface IResolveActor
	{
		Func<IServiceProvider, RequestService, object> ResolveOne { get; }
		Func<IServiceProvider, RequestService, IEnumerable<object>> ResolveMany { get; }
		Func<IServiceProvider, IServiceProvider> ReplaceContainer { get; }
	}
}