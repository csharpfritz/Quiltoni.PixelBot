using System;


namespace PixelBot.ResolverActors.Messages
{
	public class RequestService : IResolveMessage<Type>
	{
		public Type Requested { get; set; }
		public bool ResolveMany { get; set; }

		public RequestService(Type requestedType, bool resolveMany = false)
		{
			Requested = requestedType;
			ResolveMany = resolveMany;
		}
	}
}
