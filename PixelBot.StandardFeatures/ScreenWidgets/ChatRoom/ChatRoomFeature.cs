using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Extensibility;

namespace PixelBot.StandardFeatures.ScreenWidgets.ChatRoom
{

	[ActivatingEvents(StreamEvent.OnMessage)]
	public class ChatRoomFeature : BaseFeature
	{

		private readonly IHubContext<ChatRoomHub> _HubContext;

		public ChatRoomFeature(IHubContext<ChatRoomHub> hubContext) {
			_HubContext = hubContext;
		}

		public override string Name => "ChatRoom Screen Widget";

		public override void RegisterRoutes(IEndpointRouteBuilder routes) {
			
			routes.MapHub<ChatRoomHub>("/chatroom");

		}

		public override void FeatureTriggered(string notifyAction) {

			_HubContext.Clients.Group(base.Configuration.ChannelName).SendAsync(notifyAction);

		}

	}

}
