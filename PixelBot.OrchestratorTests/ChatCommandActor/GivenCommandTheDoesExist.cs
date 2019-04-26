using System;
using System.Collections.Generic;
using System.Text;
using Akka.TestKit.Xunit2;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using Xunit;
using CORE = PixelBot.Orchestrator.Actors;
using MSG = Quiltoni.PixelBot.Core.Messages;


namespace PixelBot.OrchestratorTests.ChatCommandActor
{
	public class GivenCommandTheDoesExist : BaseTestKit
	{

		public GivenCommandTheDoesExist() {

			_Args = new OnChatCommandReceivedArgs {
				Command = new ChatCommand(ChatMessageFromText("!tealoldman"))
			};

		}

		[Fact]
		public void WhenSubmittedShouldBroadcastMessage() {

			// Act
			var sut = this.Sys.ActorOf(CORE.ChannelEvents.ChatCommandActor.Props(Config));
			sut.Tell(_Args, this.TestActor);

			// Assert
			ExpectMsg<MSG.BroadcastMessage>(msg => {
				Assert.NotNull(msg.Message);
				Assert.StartsWith("TealOldMan is blowing up chat! ", msg.Message);
			});

		}



	}
}
