using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace PixelBot.Orchestrator.Actors
{
	public class FeatureManagementActor : ReceiveActor
	{

		public FeatureManagementActor(IEnumerable<Type> featureTypes) {

			this.FeatureTypes = featureTypes;

		}

		public IEnumerable<Type> FeatureTypes { get; }

		// TODO: Manage the collection of registered features
		// TODO: Allow features to be requested of an StreamEvent Type

		public static IActorRef Create(IEnumerable<Type> featureTypes) {

			var p = Props.Create<FeatureManagementActor>(featureTypes);
			return Context.ActorOf(p, "featuremanagement");

		}


	}
}
