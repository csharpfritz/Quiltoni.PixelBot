using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic.ApplicationServices;

namespace PixelBot.StandardFeatures.ScreenWidgets.ChatRoom
{
	public class ChatRoomHub : Hub
	{

		public override async Task OnConnectedAsync() {

			// Need to capture the Group / channel to listen for
			var channel = Context.GetHttpContext().Request.Query["channel"];
			await base.Groups.AddToGroupAsync(Context.ConnectionId, channel);

			await base.OnConnectedAsync();

		}

	}
}
