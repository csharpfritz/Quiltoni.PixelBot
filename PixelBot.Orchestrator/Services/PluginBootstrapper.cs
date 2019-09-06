using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PixelBot.StandardFeatures.ScreenWidgets.ChatRoom;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Extensibility;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Services
{

	public class PluginBootstrapper
	{

		private static IEnumerable<Type> _Features;
		private readonly ChannelConfiguration _Configuration;

		static PluginBootstrapper() {
			LoadFeatures();
		}

		public PluginBootstrapper(ChannelConfiguration configuration) {
			_Configuration = configuration;
		}

		public static IServiceProvider ServiceProvider { get; internal set; }

		private static void LoadFeatures() {

			var outTypeCollection = new List<Type>();

			// Extract the plugin types
			var rootAssembly = typeof(PluginBootstrapper).Assembly;
			foreach (var assembly in rootAssembly.GetReferencedAssemblies()) {

				var loadedAssembly = Assembly.Load(assembly);

				outTypeCollection.AddRange(loadedAssembly.GetTypes()
						.Where(t => typeof(IFeature).IsAssignableFrom(t) && !t.IsAbstract)); //{
			}

			_Features = outTypeCollection;

		}

		/// <summary>
		/// Initialize the features of the application.  This method should be called in Startup
		/// </summary>
		public static void InitializeFeatures(IApplicationBuilder app) {

			app.UseEndpoints(routes =>
			{

				foreach (var f in _Features)
				{

					var newFeature = ActivatorUtilities.CreateInstance(ServiceProvider, f) as IFeature;
					newFeature.RegisterRoutes(routes);

				}


			});

		}

		internal IEnumerable<IFeature> GetFeaturesForStreamEvent(StreamEvent evt) {

			var outFeatures = new List<IFeature>();

			// Identify the features that interact with the StreamEvent requested
			var featuresToMake = _Features.Where(t => {
				var attr = t.GetCustomAttributes(true)
					.Where(a => a is ActivatingEventsAttribute).FirstOrDefault() as ActivatingEventsAttribute;
				if (attr == null) return false;
				return (attr.EventsListeningTo | evt) != StreamEvent.None;
				});

			// Instantiate the features that interact with the StreamEvent requested
			foreach (var f in featuresToMake) {

				var newFeature = ActivatorUtilities.CreateInstance(ServiceProvider, f) as IFeature;
				if (!(_Configuration?.FeatureConfigurations.ContainsKey(newFeature.Name) ?? false)) continue;
				var featureConfig = _Configuration?.FeatureConfigurations[newFeature.Name];
				if (featureConfig.IsEnabled)
				{
					if (featureConfig != null) newFeature.Configure(featureConfig);
					if (featureConfig == null || newFeature.IsVisible) outFeatures.Add(newFeature);
				}

			}

			return outFeatures;

		}

	}

}
