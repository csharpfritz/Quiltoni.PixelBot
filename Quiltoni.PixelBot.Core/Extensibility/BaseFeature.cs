using System;
using Microsoft.AspNetCore.Routing;

namespace Quiltoni.PixelBot.Core.Extensibility
{
	public abstract class BaseFeature : IFeature
	{
		public abstract string Name { get; }

		public bool IsVisible { get; private set; }

		public bool IsEnabled { get; private set; }

		public Action<string> BroadcastMessage { get; set; }

		public Action<string, string> WhisperMessage { get; set; }

		protected IFeatureConfiguration Configuration { get; private set; }

		public virtual void Configure(IFeatureConfiguration configuration)
		{
			this.Configuration = configuration;
			this.IsEnabled = Configuration.IsEnabled;
			this.IsVisible = Configuration.IsVisible;
		}

		public abstract void FeatureTriggered(string notifyAction);

		public virtual void RegisterRoutes(IEndpointRouteBuilder routes) { }

	}

}