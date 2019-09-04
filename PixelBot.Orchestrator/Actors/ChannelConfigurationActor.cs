using Akka.Actor;
using PixelBot.Orchestrator.Data;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Actors
{
	public class ChannelConfigurationActor : ReceiveActor
	{
		private readonly IChannelConfigurationContext _Context;

		public static string InstancePath { get; private set; }

		public ChannelConfigurationActor(IChannelConfigurationContext context)
		{
			_Context = context;

			InstancePath = Context.Self.Path.ToStringWithAddress();

			Receive<GetConfigurationForChannel>(this.GetConfigurationForChannel);
			Receive<SaveConfigurationForChannel>(this.SaveConfigurationForChannel);

		}

		private void SaveConfigurationForChannel(SaveConfigurationForChannel msg)
		{

			_Context.SaveConfigurationForChannel(msg.ChannelName, msg.Config);
			ChannelManagerActor.Instance.Tell(new NotifyChannelOfConfigurationUpdate (msg.ChannelName, msg.Config));

		}

		private void GetConfigurationForChannel(GetConfigurationForChannel msg)
		{

			var config = _Context.GetConfigurationForChannel(msg.ChannelName);
			Context.Sender.Tell(config);

		}
	}
}
