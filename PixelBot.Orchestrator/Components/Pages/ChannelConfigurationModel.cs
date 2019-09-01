using Akka.Actor;
using Microsoft.AspNetCore.Components;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Components.Pages
{
	public class ChannelConfigurationModel : ComponentBase
	{

		[Inject]
		public IActorRef ChannelManager { get; set; }

		[CascadingParameter]
		private Task<AuthenticationState> authenticationStateTask { get; set; }


		public ChannelConfiguration Configuration { get; private set; }

		public ClaimsPrincipal User { get; private set; }

		protected override async Task OnInitializedAsync()
		{

			var User = (await authenticationStateTask).User;
			Configuration = await ChannelManager.Ask<ChannelConfiguration>(new GetConfigurationForChannel(User.Identity.Name));

			base.OnInitializedAsync();

		}

	}
}
