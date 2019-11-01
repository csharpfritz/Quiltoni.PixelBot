using System;
using System.Collections.Generic;
using System.Text;
using Akka.TestKit.Xunit2;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;
using System.Drawing;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.OrchestratorTests.ChatCommandActor
{
	public abstract class BaseTestKit : TestKit
	{

		protected OnChatCommandReceivedArgs _Args;
		protected ChannelConfiguration Config = new ChannelConfiguration
		{
			ChannelName = "thisChannel"
		};



		public static ChatMessage ChatMessageFromText(string message)
		{

			return new TestChatMessage(message);

		}

		private class TestChatMessage : ChatMessage
		{

			public TestChatMessage(string message) : base("testbot", "123456", "testuser", "Testus Userus", "#0000FF"
				, Color.Blue, null, message, UserType.Viewer, "testchannel", "theId", false, 0,
				"theRoomId", false, false, false, false, Noisy.NotSet, "", "", new List<KeyValuePair<string, string>>(),
				null, 0, 0.0D)
			{

				base.Username = "testuser";

			}

		}


	}
}