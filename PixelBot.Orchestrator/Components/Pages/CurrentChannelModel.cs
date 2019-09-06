using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public IActorRef ChannelManager { get; set; }

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		public string channelName { get; set; }

		public string[] TheCurrentChannels = new string[] { };

		protected override async Task OnAfterRenderAsync(bool firstRender) {

			if (firstRender && !(await AuthorizationService.AuthorizeAsync(CurrentUser, "GlobalAdmin")).Succeeded) {
				NavigationManager.NavigateTo("/");
				return;
			}

			// TODO: Activate the JavaScript to connect the SignalR log
			await JSRuntime.InvokeAsync<string>("StartLogging", null);

		}

		protected override async Task OnParametersSetAsync() {

			TheCurrentChannels = (await ChannelManager.Ask(new MSG.ReportCurrentChannels())) as string[];

			await base.OnParametersSetAsync();

		}

		protected async Task JoinChannel() {

			ChannelManager.Tell(new MSG.JoinChannel(channelName));
			channelName = null;

			TheCurrentChannels = (await ChannelManager.Ask(new MSG.ReportCurrentChannels())) as string[];

		}



	}
}
