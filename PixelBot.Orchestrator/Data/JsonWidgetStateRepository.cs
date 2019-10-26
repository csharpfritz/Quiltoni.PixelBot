using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quiltoni.PixelBot.Core.Data;

namespace PixelBot.Orchestrator.Data
{

	public class JsonWidgetStateRepository : IWidgetStateRepository
	{

		private readonly DirectoryInfo _StorageFolder;

		public JsonWidgetStateRepository(IWebHostEnvironment env)
		{
			_StorageFolder = new DirectoryInfo(env.ContentRootPath).GetDirectories("Configuration").First();
		}


		public async Task<Dictionary<string, string>> Get(string channelName, string widgetName)
		{

			if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentNullException(nameof(channelName));

			var configFile = _StorageFolder.GetFiles($"{channelName.ToLowerInvariant()}_{widgetName.ToLowerInvariant()}.json")
				.FirstOrDefault();

			if (configFile == null || !configFile.Exists)
			{
				return new Dictionary<string, string>();
			}

			return JsonConvert.DeserializeObject<Dictionary<string, string>>(await File.ReadAllTextAsync(configFile.FullName), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});


		}

		public async Task Save(string channelName, string widgetName, Dictionary<string, string> payload)
		{

			if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentNullException(nameof(channelName));
			if (string.IsNullOrWhiteSpace(widgetName)) throw new ArgumentNullException(nameof(widgetName));
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			var targetFile = new FileInfo(Path.Combine(_StorageFolder.FullName, $"{channelName.ToLowerInvariant()}_{widgetName.ToLowerInvariant()}.json"));
			File.WriteAllText(targetFile.FullName, JsonConvert.SerializeObject(payload, Formatting.Indented,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				}));

		}
	}

}