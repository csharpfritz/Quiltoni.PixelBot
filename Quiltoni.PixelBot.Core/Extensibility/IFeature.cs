using System;
using System.Diagnostics.Tracing;
using Microsoft.AspNetCore.Routing;
using Quiltoni.PixelBot.Core;

namespace Quiltoni.PixelBot.Core.Extensibility
{

	public interface IFeature
	{

		/// <summary>
		/// Configure the feature for a specific channel
		/// </summary>
		/// <param name="configuration">Configuration for the feature specific to a channel</param>
		/// <param name="channel">The channel to be configured</param>
		void Configure(IFeatureConfiguration configuration);

		string Name { get; }

		/// <summary>
		/// Is this feature visible to be configured on every channel?
		/// </summary>
		bool IsVisible { get; }

		/// <summary>
		/// Is this feature enabled for the current channel?
		/// </summary>
		bool IsEnabled { get; }

		void RegisterRoutes(IEndpointRouteBuilder routes);

		/// <summary>
		/// Some action on Twitch is triggering this feature
		/// </summary>
		/// <param name="notifyAction">Notification content of what triggered the feature</param>
		void FeatureTriggered(string notifyAction);

		/// <summary>
		/// Delegate that allows the feature to broadcast a message to the channel
		/// </summary>
		Action<string> BroadcastMessage { set; }

		/// <summary>
		/// Delegate that allows the feature to whisper a message to a specific chat room participant
		/// </summary>
		Action<string, string> WhisperMessage { set; }

	}

}
