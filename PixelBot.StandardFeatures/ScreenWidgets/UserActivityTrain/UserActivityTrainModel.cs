using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.ObjectPool;
using Microsoft.JSInterop;
using Quiltoni.PixelBot.Core.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain
{

	public class UserActivityTrainModel : ComponentBase, IUserActivityClient, IDisposable
	{

		// Cheer 110 ultramark 11/9/19 
		// Cheer 500 cpayette 11/9/19 

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		[Parameter]
		public string ChannelName { get; set; }

		protected override async Task OnInitializedAsync()
		{

			await JSRuntime.InvokeVoidAsync("UserActivity.Connect", args: new object[] { ChannelName, DotNetObjectReference.Create(this) });

			await base.OnInitializedAsync();

		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				JSRuntime.InvokeVoidAsync("UserActivity.Stop");

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~UserActivityTrainModel()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion

		[JSInvokable]
		public Task NewFollower(string newFollowerName)
		{
			throw new NotImplementedException();
		}

		[JSInvokable]
		public Task NewSubscriber(string newSubscribedName, int numberOfMonths, int numberOfMonthsInRow, string message)
		{
			throw new NotImplementedException();
		}

		[JSInvokable]
		public Task NewCheer(string cheererName, int amountCheered, string message)
		{
			throw new NotImplementedException();
		}

	}

}
