using System.Diagnostics.Tracing;

namespace PixelBot.Extensibility
{

	[ActivatingEvents(StreamEvent.OnMessage)]
	public interface IFeature
	{

		/// <summary>
		/// Configure the feature for a specific channel
		/// </summary>
		/// <param name="configuration">Configuration for the feature specific to a channel</param>
		/// <param name="channel">The channel to be configured</param>
		void Configure(BaseFeatureConfiguration configuration, string channel);

		string Name { get; }

		bool IsVisible { get; }

		bool IsEnabled { get; }

	}

}
