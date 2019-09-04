using Akka.Actor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using PixelBot.Orchestrator.Actors;
using PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Components.Pages
{
	public class ConfigurationModel : ComponentBase
	{

		[Inject]
		public ActorSystem ActorSystem { get; set; }

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
			var configActor = GetConfigurationActor();
			Configuration = await configActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(User.Identity.Name));

			UserActivityConfiguration = Configuration.FeatureConfigurations.GetConfigurationForFeature<UserActivityConfiguration>();

			base.OnInitializedAsync();

		}

		private IActorRef GetConfigurationActor()
		{
			return ActorSystem.ActorSelection(ChannelConfigurationActor.InstancePath).ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
		}

		public async Task SaveChanges() {

			// TODO: Validate changes
			var User = (await authenticationStateTask).User;
			var actor = GetConfigurationActor();

			// TODO: Move this assignment somewhere else... another extension method?
			Configuration.FeatureConfigurations[nameof(UserActivityConfiguration)] = UserActivityConfiguration;

			actor.Tell(new SaveConfigurationForChannel(User.Identity.Name, Configuration));

		}

	}
}
