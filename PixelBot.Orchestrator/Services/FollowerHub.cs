using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Services
{
	public class FollowerHub : Hub<IFollowerClient> {

		public override Task OnConnectedAsync()
		{

			var channelToMonitor = this.Context.GetHttpContext().Request.Query["channel"].SingleOrDefault();

			this.Groups.AddToGroupAsync(Context.ConnectionId, channelToMonitor);

			return base.OnConnectedAsync();
		}


	}

	public interface IFollowerClient
	{

		Task NewFollower(string newFollowerName);

	}

}
