using System;
using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class CloseCommandShould
    {
        [Fact]
        public void NotBeAllowedForRegularChatters()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "close",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = false,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            Assert.Throws<InvalidOperationException>(() => sut.Close(chatserviceMock.Object, cmd));

            //verify everything happened as needed
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(m => m == "Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.")), Times.Never);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]

        public void BeAllowedForBroadCastersAndModerators()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "close",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = true,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            sut.Close(chatserviceMock.Object, cmd);

            //verify everything happened as needed
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(m => m == "No more guesses...  the race is about to start with 0 guesses from 00:00:00 to 00:00:00")), Times.Once);
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }

        [Fact]
        public void BroadcastMinAndMaxGuesses()
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:23",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = false,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            sut.Guess(chatserviceMock.Object, cmd);

            cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "1:59",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = false,
                    IsModerator = false,
                    DisplayName = "User2",
                    Username = "user2"
                }
            };

            sut.Guess(chatserviceMock.Object, cmd);
            cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "close",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = true,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            sut.Close(chatserviceMock.Object, cmd);

            //verify everything happened as needed
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(m => m == "No more guesses...  the race is about to start with 2 guesses from 00:01:23 to 00:01:59")), Times.Once);
            Assert.Equal(GuessGameState.GuessesClosed, sut.CurrentState());
        }
    }
}
