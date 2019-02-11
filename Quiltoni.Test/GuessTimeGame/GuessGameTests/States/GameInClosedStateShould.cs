using System;
using System.Collections.Generic;
using System.Text;
using Quiltoni.PixelBot.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Quiltoni.Test.GuessGameTests.States
{
    public class GameInClosedStateShould
    {
        private readonly ITestOutputHelper _output;

        public GameInClosedStateShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NotAllowCloseCommand()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Assert.Throws<InvalidOperationException>(() => sut.Close(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void NotAllowResetCommandFromChatter()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Assert.Throws<InvalidOperationException>(() => sut.Reset(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { "reset", "0:00" }, ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void AllowResetCommandFromBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            sut.Reset(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(){"reset", "0:00"}, ChatUser = new ChatUser() { DisplayName = "a", Username = "a", IsBroadcaster = true} });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void AllowResetCommandFromModerator()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            sut.Reset(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { "reset", "0:00" }, ChatUser = new ChatUser() { DisplayName = "a", Username = "a", IsModerator = true } });
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void NotAllowGuessCommand()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            sut.Guess(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void AllowMineCommand()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            sut.Mine(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void NotAllowOpenCommandFromChatter()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Assert.Throws<InvalidOperationException>(() => sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>(), ChatUser = new ChatUser() { DisplayName = "a", Username = "a" } }));
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void AllowOpenCommandFromBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            sut.Open(new StubChat(_output), new GuessGameCommand() { ArgumentsAsList = new List<string>() { }, ChatUser = new ChatUser() { IsBroadcaster = true, DisplayName = "a", Username = "a" } });
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }
    }
}
