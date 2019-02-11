using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class HelpCommandShould
    {
        [Fact]
        public void WhisperTheGameIsNotStarted_WhenCommandIsTriggeredWithHelp_GivenStateIsNotStarted()
        {
            var sut = new GuessGame();
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "help",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Help(chatService.Object, cmd);
            chatService.Verify(service => service.WhisperMessage(
                It.Is<string>(u => u == "User1"),
                It.Is<string>(m => m == "The time-guessing game is not currently running.  To open the game for guesses, execute !guess open")), Times.Once);
        }

        [Fact]
        public void WhisperTheGameIsNotStarted_WhenCommandIsTriggeredWihoutArgs_GivenStateIsNotStarted()
        {
            var sut = new GuessGame();
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    //"",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.WhisperMessage(
                It.Is<string>(u => u == "User1"),
                It.Is<string>(m => m == "The time-guessing game is not currently running.  To open the game for guesses, execute !guess open")), Times.Once);
        }

        [Fact]
        public void BroadCastTheGameIsTakingGuesses_WhenTakingGuesses_GivenUserIsNotBroadcasterOrModerator()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    //"",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.BroadcastMessageOnChannel(
                It.Is<string>(m => m == "The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23  Your last guess will stand, and you can check your guess with !guess mine")), Times.Once);
        }

        [Fact]
        public void WhisperTheGameIsTakingGuesses_WhenTakingGuesses_GivenUserIsBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    //"",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.WhisperMessage(
                It.Is<string>(m=> m =="user1"),
                It.Is<string>(m => m == "The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23, Your last guess will stand, and you can check your guess with !guess mine, OR close the guesses with !guess close")), Times.Once);
        }

        [Fact]
        public void WhisperTheGameIsTakingGuesses_WhenTakingGuesses_GivenUserIsModerator()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>(),
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsModerator = true
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.WhisperMessage(
                It.Is<string>(m => m == "user1"),
                It.Is<string>(m => m == "The time-guessing game is currently taking guesses.  Guess a time with !guess 1:23, Your last guess will stand, and you can check your guess with !guess mine, OR close the guesses with !guess close")), Times.Once);
        }

        [Fact]
        public void BroadCastTheGameIsClosed_WhenClosed_GivenUserIsNotBroadcasterOrModerator()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    //"",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.BroadcastMessageOnChannel(
                It.Is<string>(m => m == "The time-guessing game is currently CLOSED.  You can check your guess with !guess mine")), Times.Once);
        }

        [Fact]
        public void WhisperNextAvailableCommands_WhenClosed_GivenUserIsBroadcasterOrModerator()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            chatserviceMock.Setup(x => x.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    //"",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Help(chatserviceMock.Object, cmd);
            chatserviceMock.Verify(service => service.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Never);
            chatserviceMock.Verify(service => service.WhisperMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
