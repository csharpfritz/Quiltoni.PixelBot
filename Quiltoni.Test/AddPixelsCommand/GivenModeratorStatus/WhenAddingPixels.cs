using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Quiltoni.PixelBot;
using TwitchLib.Client.Enums;
using Xunit;
using CMD = Quiltoni.PixelBot.Commands.AddPixelsCommand;

namespace Quiltoni.Test.AddPixelsCommand.GivenModeratorStatus
{
	public class WhenAddingPixels : BaseClass
	{

		protected override int PixelsToAdd => 50;

		[Fact]
		public void ShouldAddToGoogleSheet() {

			// arrange
			var sut = new CMD();
			var sheet = _Mockery.Create<GoogleSheetProxy>(MockBehavior.Loose);
			sheet.Setup(s => s.AddPixelsForUser(UserName, PixelsToAdd, AddCommand.ChatMessage.DisplayName))
				.Verifiable("Did not add pixels for the user");
			var twitch = _Mockery.Create<IChatService>();

			// act
			sut.GoogleSheet = sheet.Object;
			sut.Execute(base.AddCommand, twitch.Object);

			// assert
			_Mockery.Verify();

		}

	}

}
