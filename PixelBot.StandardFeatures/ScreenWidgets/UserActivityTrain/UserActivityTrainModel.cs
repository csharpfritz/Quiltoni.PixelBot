using Akka.Actor;
using Akka.Util.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.ObjectPool;
using Microsoft.JSInterop;
using Quiltoni.PixelBot.Core.Client;
using Quiltoni.PixelBot.Core.Data;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain
{

	public class UserActivityTrainModel : ComponentBase, IUserActivityClient, IDisposable
	{

		// Cheer 110 ultramark 11/9/19 
		// Cheer 500 cpayette 11/9/19 

		private const string WidgetName = "UserActivityTrainModel";

		public UserActivityTrainModel()
		{
			TrainTimer.Elapsed += TrainTimer_Elapsed;

		}

		private void TrainTimer_Elapsed(object sender, ElapsedEventArgs e)
		{

			Counter = 0;
			FirstEventTime = DateTime.MinValue;
			LastEventTime = DateTime.MinValue;

		}

		[Inject]
		public ActorSystem ActorSystem { get; set; }

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		[Parameter]
		public string ChannelName { get; set; }

		[Inject]
		public IWidgetStateRepository WidgetStateRepository { get; set; }

		public string LatestFollower { get; set; } = "";

		public UserActivityConfiguration Configuration { get; set; }

		/// <summary>
		/// The position of the train when first presented on screen
		/// </summary>
		/// <value></value>
		public decimal InitialPositionPct { get; set; } = 100.0M;

		protected override async Task OnInitializedAsync()
		{

			GetWidgetConfiguration();

			await LoadPersistedConfiguration();

			await JSRuntime.InvokeVoidAsync("UserActivity.Connect", args: new object[] { ChannelName, DotNetObjectReference.Create(this) });

			await base.OnInitializedAsync();

		}

		private async Task LoadPersistedConfiguration()
		{ 

			var payload = await WidgetStateRepository.Get(ChannelName, WidgetName);
			if (payload.Count == 0) return;
			FirstEventTime = DateTime.Parse(payload[nameof(FirstEventTime)]);
			LastEventTime = DateTime.Parse(payload[nameof(LastEventTime)]);
			Counter = (TimeRemaining.TotalSeconds > 0) ? int.Parse(payload[nameof(Counter)]) : 0;

			if (TimeRemaining.TotalSeconds > 0) {
				RestorePositionOfTrain();
				TrainTimer.Interval = TimeRemaining.TotalMilliseconds;
				TrainTimer.Start();
				await StartAnimation();
			}

		}

        private void RestorePositionOfTrain()
        {
            InitialPositionPct = 100 * (decimal)TimeRemaining.TotalSeconds / (decimal)Configuration.MaxTimeBetweenActionsInSeconds;
        }

        public void GetWidgetConfiguration()
		{
		  
			var configActorRef = ActorSystem.ActorSelection(BotConfiguration.ChannelConfigurationInstancePath).ResolveOne(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
			 
			var channelConfiguration = configActorRef.Ask<ChannelConfiguration>(new MSG.GetConfigurationForChannel(ChannelName)).GetAwaiter().GetResult();
			Configuration = channelConfiguration.FeatureConfigurations[nameof(UserActivityConfiguration)] as UserActivityConfiguration;

		}

		public Timer TrainTimer { get; set; } = new Timer();

		public int Counter { get; set; }

		private async Task StartAnimation()
		{

			await InvokeAsync(StateHasChanged);

		}

		public DateTime FirstEventTime { get; set; } = DateTime.MinValue;

		public DateTime LastEventTime { get; set; } = DateTime.MinValue;

		public TimeSpan TimeRemaining
		{
			get
			{
				return LastEventTime == DateTime.MinValue ? TimeSpan.Zero : TimeSpan.FromSeconds(
				Configuration.MaxTimeBetweenActionsInSeconds - DateTime.Now.Subtract(LastEventTime).TotalSeconds);
			}
		}

		public decimal TrainWidth {
			get { return ((100M/320M*261M) + (100M/320M*135M)*(Counter - 1));  }
		}

		/// <summary>
		/// Duration since the first entry on the train
		/// </summary>
		public TimeSpan TrainDuration
		{
			get
			{
				return FirstEventTime == DateTime.MinValue ? TimeSpan.Zero : DateTime.Now.Subtract(FirstEventTime);
			}
		}

		[JSInvokable]
		public async Task NewFollower(string newFollowerName)
		{

			// Cheer 200 goranhal 15/9/19 
			// Cheer 110 copperbeardy 20/9/19 

			if (Configuration.Type != UserActivityConfiguration.UserActivityTrainType.Follow)
			{

				return;
				 
			}
 
			// cheer 1000 jamesmontemagno 15/10/19
			// cheer 1050 tbdgamer 15/10/19

			TrainTimer.Stop();
			InitialPositionPct = 100;
			await StartAnimation();
			await Task.Delay(500);

			TrainTimer.Interval = TimeSpan.FromSeconds(Configuration.MaxTimeBetweenActionsInSeconds).TotalMilliseconds;
			if (this.Counter == 0)
			{
				FirstEventTime = DateTime.Now;
			}
			LastEventTime = DateTime.Now;
			this.Counter++;
			LatestFollower = newFollowerName;

			await WidgetStateRepository.Save(ChannelName, WidgetName, CurrentState);

			TrainTimer.Start();
			await StartAnimation();

			/**
			 * 
			 * When the timer expires, clear animation, zero-out the train?
			 * 
			 * NOTE:   Store a list of the new followers so that we don't retrigger the train for the same follower multiple times
			 * 
			 * 
			 */



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

		public Dictionary<string, string> CurrentState
		{
			get
			{
				return new Dictionary<string, string>() {
				{nameof(Counter), Counter.ToString()},
				{nameof(FirstEventTime), FirstEventTime.ToString()},
				{nameof(LastEventTime), LastEventTime.ToString()}
			};
			}
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

	}

}
