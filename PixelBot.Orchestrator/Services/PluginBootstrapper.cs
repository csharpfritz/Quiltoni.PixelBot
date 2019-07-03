using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.NETCore.Plugins;
using PixelBot.Extensibility;

namespace PixelBot.Orchestrator.Services
{
	public class PluginBootstrapper
	{

		public static IEnumerable<Type> LoadFeatures() {

			var loaders = new List<PluginLoader>();

			// create plugin loaders
			var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
			foreach (var dir in Directory.GetDirectories(pluginsDir)) {
				var dirName = Path.GetFileName(dir);
				var pluginDll = Path.Combine(dir, dirName + ".dll");
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

			return outTypeCollection;

		}

	}
}
