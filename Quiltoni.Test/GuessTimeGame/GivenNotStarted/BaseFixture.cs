using System;
using Xunit;
using Quiltoni.PixelBot.Commands;
using Moq;
using Quiltoni.PixelBot;

namespace Quiltoni.Test.GuessTimeGame.GivenNotStarted
{
	public class BaseFixture
	{

		protected BaseFixture()
		{

			this.Command = new GuessTimeCommandOld();
			GuessTimeCommandOld.State = GuessGameState.NotStarted;

			ChatService = MockRepository.Create<IChatService>();

		}

		public GuessTimeCommandOld Command { get; }

		public Mock<IChatService> ChatService { get; }

		protected MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

	}
}
