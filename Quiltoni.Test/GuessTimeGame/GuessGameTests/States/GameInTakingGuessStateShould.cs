using System;
using System.Collections.Generic;
using System.Text;
using Quiltoni.PixelBot.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Quiltoni.Test.GuessGameTests.States
{
    public class GameInTakingGuessStateShould
    {
        private readonly ITestOutputHelper _output;

        public GameInTakingGuessStateShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NotAllowCloseCommandFromChatter()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Assert.Throws<InvalidOperationException>(() => sut.Close(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void AllowCloseCommandFromBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            sut.Close(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a", IsBroadcaster = true} });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void AllowCloseCommandFromModerator()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            sut.Close(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a", IsModerator = true} });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void NotAllowResetCommand()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Assert.Throws<InvalidOperationException>(() => sut.Reset(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { "reset", "0:00" }, ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void NotAllowGuessCommand()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            sut.Guess(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void NotAllowMineCommand()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            sut.Mine(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void NotAllowOpenCommandFromChatter()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Assert.Throws<InvalidOperationException>(() => sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void AllowOpenCommandFromBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Assert.Throws<InvalidOperationException>(() => sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { }, ChatUser = new ChatUser() { IsBroadcaster = true, DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }
    }
}
