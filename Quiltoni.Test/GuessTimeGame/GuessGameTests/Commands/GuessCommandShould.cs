using System.Collections.Generic;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;

namespace Quiltoni.Test.GuessGameTests.Commands
{
    public class GuessCommandShould
    {
        [Fact]
        public void AddTheValidGuessOfAUser()
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

            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Never);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            Assert.Equal(1, sut.GuessCount());
        }

        [Fact]
        public void ReplaceAValidGuessOfTheSameUser()
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
                    "1:24",
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

            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Never);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            Assert.Equal(1, sut.GuessCount());
        }

        [Fact]
        public void AddAValidGuessOfTheDifferentUser()
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
                    "1:24",
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

            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.IsAny<string>()), Times.Never);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            Assert.Equal(2, sut.GuessCount());
        }

        [Fact]
        public void BroadcastWhenAlreadyGuessedByOtherUser()
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
                    "1:23",
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

            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(s =>s == "Sorry user2, user1 already guessed 00:01:23")), Times.Once);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            Assert.Equal(1, sut.GuessCount());
        }

        [Theory]
        [InlineData("1:61")]
        [InlineData("-1:00")]
        [InlineData("-1:-1")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("null")]
        [InlineData("1")]
        [InlineData("blah")]
        [InlineData("61:00")]
        [InlineData("!1")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("99999999999999999999")]
        [InlineData("1-56")]
        [InlineData("60:00")]
        [InlineData("1:00:00")] //we don't accept hour format only mm:ss
        [InlineData("00:00:01")]
        //[InlineData("00:00")]
        [InlineData("0:0")]
        //[InlineData("01:05")]
        [InlineData("61:*")]
        [InlineData("2/65")]
        [InlineData(";drop tables")]

        public void BroadcastInvalidGuess(string guess)
        {
            var sut = new GuessGame(GuessGameState.OpenTakingGuesses);
            Mock<IChatService> chatserviceMock = new Mock<IChatService>();
            chatserviceMock.Setup(x => x.BroadcastMessageOnChannel(It.IsAny<string>()));
            var cmd = new GuessGameCommand()
            {
                ArgumentsAsList = new List<string>()
                {
                    guess,
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
            chatserviceMock.Verify(x => x.BroadcastMessageOnChannel(It.Is<string>(s => s == "Sorry user1, guesses are only accepted in the format !guess 1:23")), Times.Once);
            Assert.Equal(GuessGameState.OpenTakingGuesses, sut.CurrentState());
            Assert.Equal(0, sut.GuessCount());
        }
    }
}
