using System;
using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class MineCommandShould
    {
        [Fact]
        public void DoNothing_WhenInNotStartedState()
        {
            var sut = new GuessGame(GuessGameState.NotStarted);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
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
            sut.Mine(chatService.Object, cmd);
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void BroadcastTheUserGuess_WhenInTakingGuessesStateAndUserDidntGuess()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "mine",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Mine(chatService.Object, cmd);
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s =>s == "user1 has not guessed yet!")), Times.Once());
        }

        [Fact]
        public void BroadcastTheUserGuess_WhenInTakingGuessesState()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Guess(chatService.Object, cmd);
            sut.Mine(chatService.Object, cmd);
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "user1 guessed 00:01:23")), Times.Once());
        }

        [Fact]
        public void BroadcastTheUserGuess_WhenInClosedStateAndUserDidntGuess()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "mine",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Mine(chatService.Object, cmd);
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "user1 has not guessed yet!")), Times.Once());
        }

        [Fact]
        public void BroadcastTheUserGuess_WhenInClosedStateAndUserGuessed()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1"
                }
            };
            sut.Guess(chatService.Object, cmd);
            sut.Close(chatService.Object, new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            });
            sut.Mine(chatService.Object, cmd);
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "user1 guessed 00:01:23")), Times.Once());
        }
    }
}
