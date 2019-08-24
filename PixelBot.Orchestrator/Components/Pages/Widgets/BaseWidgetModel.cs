using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Components;
using PixelBot.Orchestrator.Actors.ChannelEvents;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Extensibility;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Components.Pages.Widgets
{
	public abstract class BaseWidgetModel<T> : ComponentBase where T : class, IFeature
	{

		private static readonly Dictionary<StreamEvent, string> _StreamEvents = new Dictionary<StreamEvent, string> {
			{ StreamEvent.OnMessage, nameof(NewMessageActor) } 
		};

		[Inject()]
		public ActorSystem ActorSystem { get; set; }

		[Parameter]
		public string Channel { get; set; }

		public IFeature Feature { get; set; }

		public abstract StreamEvent TriggerEvent { get; }

		protected override async Task OnInitializedAsync() {

			var eventMsg = _StreamEvents[TriggerEvent];

			await InitializeFeatureForChannel(eventMsg);

			await base.OnInitializedAsync();

		}

		public async Task InitializeFeatureForChannel(string eventMsg) {
			Feature = await ActorSystem
				.ActorSelection($"/user/channelmanager/channel_{Channel}/event_{eventMsg}")
				.Ask(new GetFeatureForChannel(Channel, typeof(T))) as T;
		}
	}

}
