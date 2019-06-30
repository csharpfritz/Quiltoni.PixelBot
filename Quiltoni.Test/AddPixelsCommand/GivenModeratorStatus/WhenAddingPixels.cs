using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
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
			var sut = new CMD(Options.Create(new PixelBotConfig()));
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

		[Fact]
		public void AndCurrencyIsDisabledShouldNotAddToGoogleSheet() {

			// arrange
			var cfg = new PixelBotConfig();
			cfg.Currency.Enabled = false;
			var proxy = new Mock<ISheetProxy>();

			// Act
			var sut = new Quiltoni.PixelBot.PixelBot(
				new IBotCommand[] { }, Options.Create(cfg), 
				new NullLoggerFactory(), proxy.Object);

			sut._Client_OnNewSubscriber(null, new TwitchLib.Client.Events.OnNewSubscriberArgs());
			proxy.Verify(p => p.AddPixelsForUser(It.IsAny<string>(), 10, It.IsAny<string>()), Times.Never);


		}

	}

}
