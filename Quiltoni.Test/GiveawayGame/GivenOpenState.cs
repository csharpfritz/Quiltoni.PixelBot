using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Quiltoni.PixelBot;
using Quiltoni.PixelBot.Commands;
using Xunit;
using CORE = Quiltoni.PixelBot.GiveawayGame;

namespace Quiltoni.Test.GiveawayGame
{

	public class GivenOpenState
	{

		public Mock<IChatService> TwitchChat { get; }

		private readonly Mock<IHttpClientFactory> _MockClientFactory;
		private readonly Mock<HttpClient> _MockClient;
		private readonly CORE.GiveawayGame _Game;
		private readonly GiveawayGameCommand _Cmd;
		private PixelBot.PixelBotConfig Config = new PixelBot.PixelBotConfig {
			GiveawayGame = new CORE.GiveawayGameConfiguration {
				RelayUrl = "http://test:8000/api/Test"
			}
		};

		public GivenOpenState() {

			TwitchChat = new Mock<IChatService>();

			_MockClientFactory = new Mock<IHttpClientFactory>();
			_MockClient = new Mock<HttpClient>();
			_MockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_MockClient.Object);

			_Game = new CORE.GiveawayGame(_MockClientFactory.Object, Options.Create(Config));
			_Cmd = new GiveawayGameCommand(_Game, new Mock<IConfiguration>().Object) {
				ChatUser = new ChatUser {
					IsBroadcaster = true
				}
			};

		}

		[Fact]
		public async Task WhenStarting_ShouldBeAbleToJoinGiveaway() {

			// Arrange
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
