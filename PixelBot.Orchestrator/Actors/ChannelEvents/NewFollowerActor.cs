using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Extensibility;
using TwitchLib.Api.Services.Events.FollowerService;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{

	public class NewFollowerActor : ReceiveActor
	{

		private readonly ChannelConfiguration _Config;
		private readonly IEnumerable<IFeature> _Features;

		public NewFollowerActor(ChannelConfiguration config, IEnumerable<IFeature> features)
		{

			this._Config = config;
			this._Features = features;

			this.Receive<OnNewFollowersDetectedArgs>(args =>
			{

				foreach (var f in _Features)
				{

					foreach (var follower in args.NewFollowers)
					{

						f.FeatureTriggered($"New Follower: {follower.FromUserId}");

					}

				}

			});

		}


	}

}