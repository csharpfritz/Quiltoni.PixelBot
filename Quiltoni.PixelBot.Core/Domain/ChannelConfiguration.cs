using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Quiltoni.PixelBot.Core.Extensibility;

namespace Quiltoni.PixelBot.Core.Domain
{

	[Serializable]
	public class ChannelConfiguration
	{

		public string ChannelName { get; set; }

		public string ChannelId { get; set; }

		public bool GuessGameEnabled { get; set; } = false;

		public CurrencyConfiguration Currency { get; set; } = new CurrencyConfiguration();

		//public BaseFeatureConfiguration GetFeatureConfiguration(string featureName) {
		//	return new BaseFeatureConfiguration() {
		//		ChannelName = ChannelName,
		//		IsEnabled = true
		//	};
		//}

		// TODO: Add other configuration options later
		public Dictionary<string, BaseFeatureConfiguration> FeatureConfigurations { get; private set; } = new Dictionary<string, BaseFeatureConfiguration>();

	}


	public static class ChannelConfigurationExtensions
	{

		public static T GetConfigurationForFeature<T>(this Dictionary<string, BaseFeatureConfiguration> dictionary) where T : BaseFeatureConfiguration, new()
		{

			if (dictionary.ContainsKey(typeof(T).Name))
			{
				return (T)(dictionary[typeof(T).Name]);
			}
			return new T();

		}


	}

}