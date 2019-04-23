﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Data;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

	/// <summary>
	/// Manage a collection of Bot actors that interact with the individual channels
	/// </summary>
	public class ChannelManagerActor : ReceiveActor
	{

		private static readonly Dictionary<string, IActorRef> _ChannelActors = new Dictionary<string, IActorRef>();

		public ChannelManagerActor(IChannelConfigurationContext dataContext) {

			Logger = Context.GetLogger();

			Receive<JoinChannel>(this.GetChannelActor);
			this.DataContext = dataContext;

		}

		public IChannelConfigurationContext DataContext { get; }
		public ILoggingAdapter Logger { get; }

		private void GetChannelActor(JoinChannel msg) {

			if (_ChannelActors.ContainsKey(msg.ChannelName)) {
				Logger.Log(Akka.Event.LogLevel.InfoLevel, $"Actor for channel '{msg.ChannelName}' already present.");
				return;
			}

			var config = DataContext.GetConfigurationForChannel(msg.ChannelName);
			var props = Props.Create<ChannelActor>(config);

			var child = Context.ActorOf(props, $"channel_{msg.ChannelName}");
			_ChannelActors.Add(msg.ChannelName, child);

		}

	}
}
