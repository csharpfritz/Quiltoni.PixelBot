namespace PixelBot.ResolverActors.Messages
{
	public interface IResolveMessage<T>
	{
		T Requested { get; set; }

		bool ResolveMany { get; set; }

	}
}
