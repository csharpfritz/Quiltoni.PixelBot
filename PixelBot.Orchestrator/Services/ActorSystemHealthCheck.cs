using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using PixelBot.Orchestrator.Actors;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Services
{

	public class ActorSystemHealthCheck : IHealthCheck
	{
		private readonly ActorSystem _ActorSystem;
		private readonly IActorRef _ChannelManagerActor;
		private readonly IHostApplicationLifetime lifetime;

		public ActorSystemHealthCheck(ActorSystem system, IActorRef channelManagerActor, IHostApplicationLifetime lifetime)
		{
			this._ActorSystem = system;
			this._ChannelManagerActor = channelManagerActor;
			this.lifetime = lifetime;
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{

			if (lifetime.ApplicationStopping.IsCancellationRequested) return Task.FromResult(HealthCheckResult.Unhealthy("Stopping"));

			if (_ChannelManagerActor == null) return Task.FromResult(HealthCheckResult.Unhealthy("Actor system is not currently available"));

			var config = _ActorSystem.ActorSelection(BotConfiguration.ChannelConfigurationInstancePath).ResolveOne(TimeSpan.FromSeconds(5));
			if (config == null) return Task.FromResult(HealthCheckResult.Degraded("Configuration is not available yet"));

			return Task.FromResult(HealthCheckResult.Healthy());

		}
	}

}