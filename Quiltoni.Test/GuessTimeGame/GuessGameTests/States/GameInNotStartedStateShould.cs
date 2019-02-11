using System;
using System.Collections.Generic;
using Quiltoni.PixelBot.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Quiltoni.Test.GuessGameTests.States
{
    public class GameInNotStartedStateShould
    {
        private readonly ITestOutputHelper _output;

        public GameInNotStartedStateShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NotAllowCloseCommand()
        {
            var sut = new GuessGame();
            Assert.Throws<InvalidOperationException>(() => sut.Close(new StubChat(_output), new GuessGameCommand(){ArgumentsAsList = new List<string>(),ChatUser = new ChatUser(){DisplayName = "a", Username = "a"}}));
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void NotAllowResetCommand()
        {
            var sut = new GuessGame();
            Assert.Throws<InvalidOperationException>(() => sut.Reset(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { "reset", "0:00" }, ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void NotAllowGuessCommand()
        {
            var sut = new GuessGame();
            sut.Guess(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void NotAllowMineCommand()
        {
            var sut = new GuessGame();
            sut.Mine(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void NotAllowOpenCommandFromChatter()
        {
            var sut = new GuessGame();
            Assert.Throws<InvalidOperationException>(() => sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void AllowOpenCommandFromBroadcaster()
        {
            var sut = new GuessGame();
            sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(){ }, ChatUser = new ChatUser() {IsBroadcaster = true,DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }
    }
}
