using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Quiltoni.Test
{
	public class GuessGameShould
    {
        private readonly ITestOutputHelper _output;

        public GuessGameShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GamePlay()
        {
            var sut = new GuessGame();
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            sut.Help(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "help" },
                ChatUser = new ChatUser() { IsModerator = true, IsBroadcaster = false, DisplayName = "bravecobra", Username = "bravecobra2" }
            });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            sut.Open(new StubChat(_output), new GuessGameCommand(){
                ArgumentsAsList = new List<string>(){ "open" },
                ChatUser = new ChatUser(){IsModerator = true, IsBroadcaster = false, DisplayName = "bravecobra", Username = "bravecobra2"}});
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "1:23" },
                ChatUser = new ChatUser() { IsModerator = true, IsBroadcaster = false, DisplayName = "bravecobra", Username = "bravecobra2" }
            });
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "mine" },
                ChatUser = new ChatUser() { IsModerator = true, IsBroadcaster = false, DisplayName = "bravecobra", Username = "bravecobra2" }
            });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "1:22" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = true, DisplayName = "csharpfritz", Username = "csharpfritz" }
            });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "1:61" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = false, DisplayName = "someone", Username = "someone" }
            });
            sut.Mine(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "mine" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = false, DisplayName = "someone", Username = "someone" }
            });
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "1:41" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = false, DisplayName = "someone", Username = "someone" }
            });
            sut.Mine(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "mine" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = false, DisplayName = "someone", Username = "someone" }
            });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Guess(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "1:25" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = false, DisplayName = "someone", Username = "someone" }
            });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Close(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "close" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = true, DisplayName = "csharpfritz", Username = "csharpfritz" }
            });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
            sut.Open(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "open" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = true, DisplayName = "csharpfritz", Username = "csharpfritz" }
            });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            sut.Close(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "close" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = true, DisplayName = "csharpfritz", Username = "csharpfritz" }
            });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
            sut.Reset(new StubChat(_output), new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>() { "reset", "1:23" },
                ChatUser = new ChatUser() { IsModerator = false, IsBroadcaster = true, DisplayName = "csharpfritz", Username = "csharpfritz" }
            });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }
    }
}
