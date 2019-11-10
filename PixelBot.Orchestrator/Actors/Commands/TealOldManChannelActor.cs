using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Messages;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.Commands
{

	public class TealOldManCommandActor : ReceiveActor, IBotCommandActor
	{

		public TealOldManCommandActor()
		{

			Receive<OnChatCommandReceivedArgs>(_ => Sender.Forward(OutMessage));

		}

		public string CommandText => "tealoldman";

		public static readonly BroadcastMessage OutMessage =
			new BroadcastMessage("TealOldMan is blowing up chat!  EVERYTHING is now blue!");


	}

}