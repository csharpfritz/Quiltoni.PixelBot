using System;
using System.Collections.Generic;
using Akka.Actor;
using PixelBot.Orchestrator.Actors.Commands;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Extensibility;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class ChatCommandActor : ReceiveActor
	{

		private readonly Dictionary<string, IActorRef> _Commands = new Dictionary<string, IActorRef>();

		public ChannelConfiguration Config { get; }

		public ChatCommandActor(ChannelConfiguration config, IEnumerable<IFeature> features) {

			this.Config = config;
			Receive<OnChatCommandReceivedArgs>(cmd => OnChatCommandReceived(cmd));

		}

		private void OnChatCommandReceived(OnChatCommandReceivedArgs args) {

			IActorRef thisCommand = null;
			if (!_Commands.ContainsKey(args.Command.CommandText)) {    // !tealoldman
				thisCommand = CreateActorForCommand(args.Command.CommandText);

				if (thisCommand is null) {

					Context.Sender.Tell(new WhisperMessage(args.Command.ChatMessage.Username, $"Unknown command '{args.Command.CommandText}' - use !help to get a list of valid commands"));
					return;

				}
				else {
					_Commands.Add(args.Command.CommandText, thisCommand);
				}

			}
			else {
				thisCommand = _Commands[args.Command.CommandText];
			}

			thisCommand.Tell(args, Context.Sender);

		}

		private IActorRef CreateActorForCommand(string commandText) {
			
			// TODO: Determine best way to identify, create, and load various command actors

			switch (commandText) {
				case "tealoldman":
					return Context.ActorOf<TealOldManCommandActor>();
				case "guess":
					return Config.GuessGameEnabled ? Context.ActorOf<GuessGameCommandActor>()
						: null;
				case "add":
					return Config.Currency.Enabled ? AddCurrencyCommandActor.CreateActor(Config)
						: null;
			}

			if (Config.Currency.Enabled && commandText == "my" + Config.Currency.Name)
				return MyCurrencyCommandActor.CreateActor(Config);

			return null;

		}

		public static Props Props(ChannelConfiguration config, IEnumerable<IFeature> features = null) {
			return Akka.Actor.Props.Create<ChatCommandActor>(config, features);
		}

	}

}
