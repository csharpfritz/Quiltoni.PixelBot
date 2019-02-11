using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class ResetCommandShould
    {
        [Fact]
        public void SetTheGameToNotStartedState_GivenUserIsBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.GuessesClosed);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "reset",
                    "1:23"
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Reset(chatService.Object, cmd);
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            Assert.Equal(0, sut.GuessCount());
            //chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "user1 has not guessed yet!")), Times.Once());
        }
        
        [Fact]
        public void AnnounceWinner_GivenUserIsBroadcaster()
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
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Guess(chatService.Object, cmd);
            sut.Close(chatService.Object, cmd);
            cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "reset",
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Reset(chatService.Object, cmd);
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "WINNER!!! - Congratulations user1 - you have won!")), Times.Once());
        }

        [Fact]
        public void AnnounceAClosestWinner_GivenUserIsBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:24",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Guess(chatService.Object, cmd);
            sut.Close(chatService.Object, cmd);
            cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "reset",
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Reset(chatService.Object, cmd);
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.Is<string>(s => s == "No winners THIS time, but user1 was closest at just 1 seconds off!")), Times.Once());
        }

        [Fact]
        public void JustReset_GivenUserIsBroadcaster()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatService = new Mock<IChatService>();
            chatService.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:24",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Guess(chatService.Object, cmd);
            sut.Close(chatService.Object, cmd);
            cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "reset",
                    "invalid",
                },
                ChatUser = new ChatUser()
                {
                    DisplayName = "User1",
                    Username = "user1",
                    IsBroadcaster = true
                }
            };
            sut.Reset(chatService.Object, cmd);
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
            chatService.Verify(service => service.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Once());
        }
    }
}
