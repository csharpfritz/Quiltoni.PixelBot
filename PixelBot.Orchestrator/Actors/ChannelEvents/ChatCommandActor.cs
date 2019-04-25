using System;
using System.Collections.Generic;
using Akka.Actor;
using PixelBot.Orchestrator.Actors.Commands;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class ChatCommandActor : ReceiveActor
	{

		private readonly Dictionary<string, IActorRef> _Commands = new Dictionary<string, IActorRef>();

		public ChatCommandActor(ChannelConfiguration config) {

			Receive<OnChatCommandReceivedArgs>(cmd => OnChatCommandReceived(cmd));
			

		}

		private void OnChatCommandReceived(OnChatCommandReceivedArgs cmd) {

			IActorRef thisCommand = null;
			if (!_Commands.ContainsKey(cmd.Command.CommandText)) {    // !tealoldman
				thisCommand = CreateActorForCommand(cmd.Command.CommandText);

				if (thisCommand is null) {

					Context.Parent.Tell(new WhisperMessage(cmd.Command.ChatMessage.Username, $"Unknown command '{cmd.Command.CommandText}' - use !help to get a list of valid commands"));
					return;

				}
				else {
					_Commands.Add(cmd.Command.CommandText, thisCommand);
				}

			}
			else {
				thisCommand = _Commands[cmd.Command.CommandText];
			}

			thisCommand.Tell(cmd, Context.Parent);

		}

		private IActorRef CreateActorForCommand(string commandText) {
			
			switch (commandText) {
				case "tealoldman":
					return Context.ActorOf<TealOldManCommandActor>();
			}

			return null;

		}

	}

}
