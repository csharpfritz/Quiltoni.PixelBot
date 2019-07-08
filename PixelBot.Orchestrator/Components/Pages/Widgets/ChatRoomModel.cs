using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Components;
using PixelBot.StandardFeatures.ScreenWidgets.ChatRoom;
using Quiltoni.PixelBot.Core.Extensibility;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Components.Pages.Widgets
{
	public class ChatRoomModel : ComponentBase
	{

		[Inject()]
		public ActorSystem ActorSystem { get; set; }

		[Parameter]
		public string Channel { get; set; }

		public IFeature ChatRoomFeature { get; set; }

		protected override async Task OnInitAsync() {

			ChatRoomFeature = await ActorSystem.ActorSelection($"/user/channelmanager/channel_{Channel}/event_NewMessageActor")
				.Ask(new GetFeatureForChannel(Channel, typeof(ChatRoomFeature))) as ChatRoomFeature;

			base.OnInitAsync();

		}

	}
}
