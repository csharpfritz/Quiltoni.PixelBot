using Akka.Actor;
using Microsoft.AspNetCore.Components;
using PixelBot.Orchestrator.Actors;
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

		public ClaimsPrincipal User { get; private set; }

		protected override async Task OnInitializedAsync()
		{

			var User = (await authenticationStateTask).User;
			var configActor = ActorSystem.ActorSelection(ChannelConfigurationActor.InstancePath).ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
			Configuration = await configActor.Ask<ChannelConfiguration>(new GetConfigurationForChannel(User.Identity.Name));

			base.OnInitializedAsync();

		}

	}
}
