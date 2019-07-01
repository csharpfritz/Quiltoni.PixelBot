using System;
using System.Collections.Generic;
using System.Linq;
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

	public class GivenOpenState
	{

		public Mock<IChatService> TwitchChat { get; }

		private readonly Mock<HttpMessageHandler> _MockHandler = new Mock<HttpMessageHandler>();
		private readonly IHttpClientFactory _MockClientFactory;
		private readonly CORE.GiveawayGame _Game;
		private readonly GiveawayGameCommand _Cmd;
		private PixelBot.PixelBotConfig Config = new PixelBot.PixelBotConfig {
			GiveawayGame = new CORE.GiveawayGameConfiguration {
				RelayUrl = "http://test:8000/api/Test"
			}
		};

		public GivenOpenState() {

			TwitchChat = new Mock<IChatService>();

			_MockClientFactory = _MockHandler.CreateClientFactory();

			_Game = new CORE.GiveawayGame(_MockClientFactory, Options.Create(Config));
			_Cmd = new GiveawayGameCommand(_Game, new Mock<IConfiguration>().Object) {
				ChatUser = new ChatUser {
					IsBroadcaster = true
				}
			};

		}

		[Fact]
		public async Task WhenStarting_ShouldBeAbleToJoinGiveaway() {

			// Arrange
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });

			TwitchChat.Setup(t => t.BroadcastMessageOnChannel(It.Is<string>(s => s.StartsWith("Giveaway starting in "))));

			// Act
			_Game.ClearEntrants();
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.EnterGiveaway("1");
			var theTask = Task.Run(() => _Game.Start(TwitchChat.Object, _Cmd, 5));
			_Game.EnterGiveaway("2");
			await theTask.ContinueWith(_ => {

				// Assert
				TwitchChat.VerifyAll();
				Assert.Equal(CORE.GiveawayGameState.Running, _Game.State);
				Assert.NotEmpty(_Game.Entrants);
				Assert.Equal(2, _Game.Entrants.Count());

			});


		}

		[Fact]
		public async Task WhenStarting_ShouldChangeStateToRunning() {

			// Arrange
			_Game.EnableCountdownTimer = false;

			// Act
			_Game.ClearEntrants();
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.Start(TwitchChat.Object, _Cmd);

			// Assert
			Assert.NotEqual(CORE.GiveawayGameState.Open, _Game.State);


		}

		[Fact]
		public void WhenSameChatterEntersTwice_ShouldOnlyBeAddedOnce() {

			// Arrange
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });
			_MockHandler.SetupRequest(HttpMethod.Put, Config.GiveawayGame.RelayUrl, msg => msg.Method == HttpMethod.Put)
				.ReturnsResponse(HttpStatusCode.OK, _ => new HttpResponseMessage { });


			// Act
			_Game.ClearEntrants();
			_Game.Open(TwitchChat.Object, _Cmd);
			_Game.EnterGiveaway("1");
			_Game.EnterGiveaway("1");

			// Assert
			Assert.NotEmpty(_Game.Entrants);
			Assert.Single(_Game.Entrants);


		}

	}

}
