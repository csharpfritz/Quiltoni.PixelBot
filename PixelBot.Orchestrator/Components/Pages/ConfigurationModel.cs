using Akka.Actor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Actors;
using PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Components.Pages
{
	public class ConfigurationModel : ComponentBase
	{

		[Inject]
		public ActorSystem ActorSystem { get; set; }

		[Inject]
		public IActorRef ChannelManager { get; set; }

		[Inject]
		public ILoggerFactory LoggerFactory {get; set;}

		[CascadingParameter]
		private Task<AuthenticationState> authenticationStateTask { get; set; }


		public ChannelConfiguration Configuration { get; private set; }

		public UserActivityConfiguration UserActivityConfiguration { get; set; }

		public ClaimsPrincipal User { get; private set; }

		protected override async Task OnInitializedAsync()
		{

			// Cheer 500 faniereynders 03/9/19
			// Cheer 550 tbdgamer 04/9/19

			var User = (await authenticationStateTask).User;
			//doe
			var configActor = GetConfigurationActor();
			Configuration = await configActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(User.Identity.Name));

			UserActivityConfiguration = Configuration.FeatureConfigurations.GetConfigurationForFeature<UserActivityConfiguration>();

			base.OnInitializedAsync();

		}

		private IActorRef GetConfigurationActor()
		{
			return ActorSystem.ActorSelection(ChannelConfigurationActor.InstancePath).ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
		}

		public async Task SaveChanges()
		{

			// Cheer 642 cpayette 05/9/19

			// TODO: Validate changes
			var User = (await authenticationStateTask).User;
			var actor = GetConfigurationActor();

			// TODO: Move this assignment somewhere else... another extension method?
			Configuration.FeatureConfigurations[nameof(UserActivityConfiguration)] = UserActivityConfiguration;

			actor.Tell(new SaveConfigurationForChannel(User.Identity.Name, Configuration));

		}

		public async Task JoinChannel()
		{

			// cheer 600 cpayette 17/10/2019
			// cheer 1000 faniereynders 17/10/2019
			// cheer 500 roberttables 17/10/2019 - [containers, containers, containers]
			// cheer 400 cpayette 18/10/2019
		
			var User = (await authenticationStateTask).User;
			ChannelManager.Tell(new MSG.JoinChannel(User.Identity.Name));

			await Task.CompletedTask;

		}

	}
}
