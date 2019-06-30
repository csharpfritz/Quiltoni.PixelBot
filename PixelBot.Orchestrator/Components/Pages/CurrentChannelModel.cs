using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Components.Pages
{
	public class CurrentChannelModel : ComponentBase
	{

		[Inject]
		public IAuthorizationService AuthorizationService { get; set; }

		[Inject]
		public ClaimsPrincipal CurrentUser { get; set; }

		[Inject]
		public IUriHelper UriHelper { get; set; }

		[Inject]
		public IActorRef ChannelManager { get; set; }

		public string channelName { get; set; }

		public string[] TheCurrentChannels = new string[] { };

		protected override async Task OnAfterRenderAsync() {

			if (!(await AuthorizationService.AuthorizeAsync(CurrentUser, "GlobalAdmin")).Succeeded) {
				UriHelper.NavigateTo("/");
				return;
			}

			TheCurrentChannels = (await ChannelManager.Ask(new MSG.ReportCurrentChannels())) as string[];

		}

		protected async Task JoinChannel() {

			ChannelManager.Tell(new MSG.JoinChannel(channelName));
			channelName = null;

			TheCurrentChannels = (await ChannelManager.Ask(new MSG.ReportCurrentChannels())) as string[];

		}



	}
}
