using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

		static PluginBootstrapper() {
			LoadFeatures();
		}

		public PluginBootstrapper() { }

		private static void LoadFeatures() {

			var outTypeCollection = new List<Type>();

			// Extract the plugin types
			var rootAssembly = typeof(PluginBootstrapper).Assembly;
			foreach (var assembly in rootAssembly.GetReferencedAssemblies()) {

				if (assembly.FullName.Contains("StandardFeatures")) Debugger.Break();
				var loadedAssembly = Assembly.Load(assembly);

				outTypeCollection.AddRange(loadedAssembly.GetTypes()
						.Where(t => typeof(IFeature).IsAssignableFrom(t) && !t.IsAbstract)); //{
			}

			_Features = outTypeCollection;

		}

		internal IEnumerable<IFeature> GetFeaturesForStreamEvent(StreamEvent evt, ChannelConfiguration config) {

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

				var newFeature = Activator.CreateInstance(f) as IFeature;
				var featureConfig = config.GetFeatureConfiguration(newFeature.Name);
				newFeature.Configure(featureConfig);
				if (newFeature.IsVisible) outFeatures.Add(newFeature);

			}

			return outFeatures;

		}

	}

}
