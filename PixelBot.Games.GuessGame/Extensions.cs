using TwitchLib.Client.Models;

namespace PixelBot.Games.GuessGame
{
	public static class Extensions
	{

		public static ChatUser AsChatUser(this ChatMessage message) {

			return new ChatUser {
				IsBroadcaster = message.IsBroadcaster,
				IsModerator = message.IsModerator,
				Username = message.Username,
				DisplayName = message.DisplayName
			};

		}

		public static GuessGameCommand AsGuessGameCommand(this ChatCommand cmd) {

			return new GuessGameCommand {

				ArgumentsAsList = cmd.ArgumentsAsList,
				ChatUser = cmd.ChatMessage.AsChatUser()

			};


		}

	}

}
