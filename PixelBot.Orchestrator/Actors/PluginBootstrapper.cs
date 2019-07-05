using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using McMaster.NETCore.Plugins;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Extensibility;
using Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.Orchestrator.Actors
{

	public class PluginBootstrapper : ReceiveActor
	{

		public const string Name = "pluginbootstrapper";
		private readonly IServiceProvider _Provider;

		public static string Path { get; private set; }

		private IEnumerable<Type> _Features;

		public PluginBootstrapper(IServiceProvider provider) {

			_Provider = provider;

			LoadFeatures();

      // TODO: Write message handlers to receive requests for Types and send messages about those types
      this.Receive<RequestFeaturesForStreamEvent>(e => {
				var features = GetFeaturesForStreamEvent(e.StreamEvent).ToArray();
				Sender.Tell(new SendFeatures(features)); 
			});


    }

		private void LoadFeatures() {

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

		private IEnumerable<IFeature> GetFeaturesForStreamEvent(StreamEvent evt) {

			var outFeatures = new List<IFeature>();

			var featuresToMake = _Features.Where(t => {
				var attr = t.GetCustomAttributes(true)
					.Where(a => a is ActivatingEventsAttribute).FirstOrDefault() as ActivatingEventsAttribute;
				if (attr == null) return false;
				return (attr.EventsListeningTo | evt) != StreamEvent.None;
				});

			foreach (var f in featuresToMake) {
				var newFeature = _Provider.GetService(f) as IFeature;
				//newFeature.Configure()
				outFeatures.Add(newFeature);
			}

			return outFeatures;

		}

		internal static IActorRef Create(IServiceProvider serviceProvider) {

			var props = Akka.Actor.Props.Create<PluginBootstrapper>(serviceProvider);
			return Context.ActorOf(props, Name);

		}

	}
}
