namespace PixelBot.ResolverActors.Messages
{
	public class ReplaceContainer<Container>
	{
		public ReplaceContainer(Container replacementContainer)
		{
			ReplacementContainer = replacementContainer;
		}

		public Container ReplacementContainer { get; }
	}
}
