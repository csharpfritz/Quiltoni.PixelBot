using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using Xunit;

namespace Quiltoni.Test.GuessTimeGame.GivenNotStarted
{
	public class WhenHelpCommandReceived : BaseFixture
	{

		public WhenHelpCommandReceived() : base()
		{

		}

		[Fact]
		public void ForViewersShouldNotReportText()
		{

			// Arrange

			// Act
//			ChatService.Setup(sut => sut.BroadcastMessageOnChannel(It.Is<string>(s => s.Contains(" not currently running "))));
			Command.Execute(new ChatCommand(GetHelpMessageForUserType(UserType.Viewer)), ChatService.Object);

			// Assert
			ChatService.Verify(
				sut => sut.BroadcastMessageOnChannel(It.Is<string>(s => s.Contains(" not currently running."))
				), Times.Never, "Should not have output help message");

		}

		[Fact]
		public void ForBroadcastersShouldReportText()
		{

			// Arrange

			// Act
			var cmd = new ChatCommand(GetHelpMessageForUserType(UserType.Broadcaster));
			Command.Execute(cmd, ChatService.Object);

			// Assert
			ChatService.Verify(
				sut => sut.WhisperMessage(cmd.ChatMessage.DisplayName, It.Is<string>(s => s.Contains(" not currently running."))
				), Times.Once, "Did not output help message");

		}

		[Fact]
		public void ForModeratorsShouldReportText()
		{

			// Arrange
			var cmd = new ChatCommand(GetHelpMessageForUserType(UserType.Moderator));
			ChatService.Setup(
				sut => sut.WhisperMessage(cmd.ChatMessage.DisplayName, It.Is<string>(s => s.Contains(" not currently running."))
				))
				.Verifiable("Did not output help message");

			// Act
			Command.Execute(cmd, ChatService.Object);

			// Assert
			ChatService.VerifyAll();

		}

		private ChatMessage GetHelpMessageForUserType(UserType type)
		{

			return new ChatMessage("quiltonibot", "123456789", "testUserName", "Test User Name", "#FF0000",
					System.Drawing.Color.Red, null, "!guess help", TwitchLib.Client.Enums.UserType.Viewer, "#testChannel", "1234",
					false, 0, "123", false, type==UserType.Moderator, false, type==UserType.Broadcaster, TwitchLib.Client.Enums.Noisy.False, "!guess help",
					"", null, null, 0, 0D);

		}

	}

}