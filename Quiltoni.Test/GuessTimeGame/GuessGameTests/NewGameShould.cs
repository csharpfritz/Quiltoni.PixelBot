using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests
{
    public class NewGameShould
    {
        [Fact]
        public void StartInNotStartedState_WhenGivenNoConstructorArguments()
        {
            var sut = new GuessGame();
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void StartInState_WhenStatePassedToConstructor()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());

            sut = new GuessGame(GuessGameState.GuessesClosed);
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }
    }
}
