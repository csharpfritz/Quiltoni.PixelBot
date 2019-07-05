using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using McMaster.NETCore.Plugins;
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

		public PluginBootstrapper() {

   //   // TODO: Write message handlers to receive requests for Types and send messages about those types
   //   this.Receive<RequestFeaturesForStreamEvent>(e => {
			//	var features = GetFeaturesForStreamEvent(e.StreamEvent).ToArray();
			//	Sender.Tell(new SendFeatures(features), Self); 
			//});


    }

		private static void LoadFeatures() {

			var loaders = new List<PluginLoader>();

			// create plugin loaders
			var pluginsDir = System.IO.Path.Combine(AppContext.BaseDirectory, "plugins");
			if (!Directory.Exists(pluginsDir)) return;

			foreach (var dir in Directory.GetDirectories(pluginsDir)) {
				var dirName = System.IO.Path.GetFileName(dir);
				var pluginDll = System.IO.Path.Combine(dir, dirName + ".dll");
				if (File.Exists(pluginDll)) {
					var loader = PluginLoader.CreateFromAssemblyFile(
							pluginDll,
							sharedTypes: new[] { typeof(IFeature) });
					loaders.Add(loader);
				}
			}

			var outTypeCollection = new List<Type>();

			// Extract the plugin types
			foreach (var loader in loaders) {

				outTypeCollection.AddRange(loader
					.LoadDefaultAssembly().GetTypes()
						.Where(t => typeof(IFeature).IsAssignableFrom(t) && !t.IsAbstract)); //{
			}

			_Features = outTypeCollection;

		}

		internal IEnumerable<IFeature> GetFeaturesForStreamEvent(StreamEvent evt, ChannelConfiguration config) {

			var outFeatures = new List<IFeature>();

			var featuresToMake = _Features.Where(t => {
				var attr = t.GetCustomAttributes(true)
					.Where(a => a is ActivatingEventsAttribute).FirstOrDefault() as ActivatingEventsAttribute;
				if (attr == null) return false;
				return (attr.EventsListeningTo | evt) != StreamEvent.None;
				});

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
