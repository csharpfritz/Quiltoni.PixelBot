using Microsoft.AspNetCore.SignalR;
using Quiltoni.PixelBot.Core.Client;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Services
{
	public class UserActivityHub : Hub<IUserActivityClient>
	{

		public override Task OnConnectedAsync()
		{

			var channelToMonitor = this.Context.GetHttpContext().Request.Query["channel"].SingleOrDefault();

			this.Groups.AddToGroupAsync(Context.ConnectionId, channelToMonitor);

			return base.OnConnectedAsync();
		}


	}

}