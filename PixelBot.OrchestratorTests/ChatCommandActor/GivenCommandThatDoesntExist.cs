using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Akka.TestKit.Xunit2;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using Xunit;
using CORE = PixelBot.Orchestrator.Actors;
using TWITCH = TwitchLib.Client.Models;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.OrchestratorTests.ChatCommandActor
{

	public class GivenCommandThatDoesntExist : BaseTestKit
	{
		private string CommandThatDoesntExist = "!TestCommandThatDoesntExist";

		public GivenCommandThatDoesntExist()
		{

			_Args = new OnChatCommandReceivedArgs
			{
				Command = new TWITCH.ChatCommand(ChatMessageFromText(CommandThatDoesntExist))
			};

		}

		[Fact]
		public void WhenSubmittedShouldWhisperCommandNotFound()
		{

			// Act
			var sut = this.Sys.ActorOf(CORE.ChannelEvents.ChatCommandActor.Props(Config));
			sut.Tell(_Args, this.TestActor);

			// Assert
			ExpectMsg<MSG.WhisperMessage>(msg =>
			{
				Assert.NotNull(msg.Message);
				Assert.Equal("testuser", msg.UserToWhisper);
				Assert.StartsWith("Unknown command ", msg.Message);
			});

		}


	}

}