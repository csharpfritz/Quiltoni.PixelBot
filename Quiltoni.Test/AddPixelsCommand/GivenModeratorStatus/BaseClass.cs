using System;
using System.Collections.Generic;
using Moq;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace Quiltoni.Test.AddPixelsCommand.GivenModeratorStatus
{
	public class BaseClass
	{

		protected virtual UserType UserType => UserType.Moderator;

		protected virtual string UserName { get; }

		protected virtual int PixelsToAdd { get; }

		protected readonly MockRepository _Mockery;

		protected BaseClass() {
			_Mockery = new MockRepository(MockBehavior.Loose);
			UserName = "testUser" + new Random().Next(0, int.MaxValue);
		}

		protected ChatCommand AddCommand {
			get {

				var cmd = new ChatCommand(new ChatMessage("testBot", "1234", "testUser", "Test User", "#F00", System.Drawing.Color.Red,
					null, "the message", UserType, "#testChannel", "id", false, 0, "room", false, UserType == UserType.Moderator,
					false, UserType == UserType.Broadcaster, TwitchLib.Client.Enums.Noisy.False, "irc stuff", "emote message", null, null, 0, 0D),
					$"!add {UserName} {PixelsToAdd}", $"{UserName} {PixelsToAdd}", new List<string> { {UserName }, { PixelsToAdd.ToString() }}, '!');

				typeof(ChatMessage).GetProperty("Username").SetValue(cmd.ChatMessage, "testUser");

				return cmd;

			}
		}

	}

}
