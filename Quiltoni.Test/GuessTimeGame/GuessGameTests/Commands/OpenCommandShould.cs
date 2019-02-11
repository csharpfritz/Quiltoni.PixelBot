using System;
using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class OpenCommandShould
    {

        [Fact]
        public void TransitionToTakingGuessesAndBroadcast_WhenOpenIsTriggered_GivenStateIsNotStartedAndUserIsModerator()
        {
            var sut = new GuessGame(GuessGameState.NotStarted);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "open",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = true,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            sut.Open(chatserviceMock.Object, cmd);

            //verify everything happened as needed
            chatserviceMock.Verify(x =>x.BroadcastMessageOnChannel(It.Is<string>(m =>m == "Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.")));
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }

        [Fact]
        public void NotBeAllowedForRegularChatters()
        {
            var sut = new GuessGame(GuessGameState.NotStarted);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "open",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = false,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            Assert.Throws<InvalidOperationException>(() => sut.Open(chatserviceMock.Object, cmd)) ;

            //verify everything happened as needed
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(m => m == "Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.")), Times.Never);
            Assert.Equal(GuessGameState.NotStarted, sut.CurrentState());
        }

        [Fact]
        public void BeAllowedForBroadcastersAndModerators()
        {
            var sut = new GuessGame(GuessGameState.NotStarted);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    "open",
                },
                ChatUser = new ChatUser()
                {
                    IsBroadcaster = true,
                    IsModerator = false,
                    DisplayName = "User1",
                    Username = "user1"
                }
            };

            sut.Open(chatserviceMock.Object, cmd);

            //verify everything happened as needed
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(m => m == "Now taking guesses. Submit your guess with !guess 1:23 where 1 is minutes and 23 is seconds.")), Times.Once);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
        }
    }
}
