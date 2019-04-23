using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;
using CORE = Quiltoni.PixelBot.GiveawayGame;

namespace Quiltoni.Test.GiveawayGame
{

	public class GivenASelectedWinner
	{

		private readonly IHttpClientFactory _MockClientFactory;
		private readonly Mock<HttpMessageHandler> _MockHandler = new Mock<HttpMessageHandler>();
		private readonly CORE.GiveawayGame _Game;
		private readonly GiveawayGameCommand _Cmd;
		private PixelBot.PixelBotConfig Config = new PixelBot.PixelBotConfig {
			GiveawayGame = new CORE.GiveawayGameConfiguration {
				RelayUrl = "http://test:8000/api/Test"
			}
		};

		public GivenASelectedWinner() {


			TwitchChat = new Mock<IChatService>();

			_MockClientFactory = _MockHandler.CreateClientFactory();

			_Game = new CORE.GiveawayGame(_MockClientFactory, Options.Create(Config));
			_Cmd = new PixelBot.Commands.GiveawayGameCommand(_Game, new Mock<IConfiguration>().Object) {
				ChatUser = new PixelBot.Commands.ChatUser {
					IsBroadcaster = true
				}
			};


		}

		public Mock<IChatService> TwitchChat { get; }

		[Fact]
		public void ShouldReportTheWinner() {

			// Arrange
			_Game.EnableCountdownTimer = false;
			var entrants = new[] { "testUser" };
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage {});
			_MockHandler.SetupRequest(HttpMethod.Post, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Post)
				.ReturnsResponse(HttpStatusCode.OK, new StringContent("testUser"));
			TwitchChat.Setup(t => t.BroadcastMessageOnChannel($"Congratulation @testUser - you have won the raffle!"));

			// Act
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.EnterGiveaway("1");
			_Game.Start(TwitchChat.Object, _Cmd);

			// Assert
			Assert.Equal(CORE.GiveawayGameState.Idle, _Game.State);
			TwitchChat.VerifyAll();

		}

		[Fact]
		public void AttemptingToRerunBeforeAnnouncementShouldReturnErrorMessage() {

			// Arrange
			_Game.EnableCountdownTimer = true;
			var entrants = new[] { "testUser" };
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });
			_MockHandler.SetupRequest(HttpMethod.Post, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Post)
				.ReturnsResponse(HttpStatusCode.OK, new StringContent("testUser"));


			// Act
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.EnterGiveaway("1");
			_Game.Start(TwitchChat.Object, _Cmd, 2);

			Task.Delay(500).GetAwaiter().GetResult();
			_Game.Start(TwitchChat.Object, _Cmd);

			// Assert
			Assert.Equal(CORE.GiveawayGameState.Running, _Game.State);
			TwitchChat.Verify(t => t.WhisperMessage(_Cmd.ChatUser.Username, "Game is not ready to be re-started.  Wait for the winner to be announced before executing !giveaway start"));

		}
		[Fact]
		public void AttemptingToOpenBeforeAnnouncementShouldReturnErrorMessage() {

			// Arrange
			_Game.EnableCountdownTimer = true;
			var entrants = new[] { "testUser" };
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });
			_MockHandler.SetupRequest(HttpMethod.Post, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Post)
				.ReturnsResponse(HttpStatusCode.OK, new StringContent("testUser"));


			// Act
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.EnterGiveaway("1");
			_Game.Start(TwitchChat.Object, _Cmd, 2);

			Task.Delay(500).GetAwaiter().GetResult();
			_Game.Open(TwitchChat.Object, _Cmd);

			// Assert
			Assert.Equal(CORE.GiveawayGameState.Running, _Game.State);
			TwitchChat.Verify(t => t.WhisperMessage(_Cmd.ChatUser.Username, "Game is not ready to be re-opened.  Wait for the winner to be announced before executing !giveaway open"));

		}

	}

}
