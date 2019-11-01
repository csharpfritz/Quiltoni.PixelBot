using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Data
{
	public class FileStorageChannelConfigurationContext : IChannelConfigurationContext
	{

		private readonly DirectoryInfo _StorageFolder;

		public FileStorageChannelConfigurationContext(IWebHostEnvironment env)
		{

			_StorageFolder = new DirectoryInfo(env.ContentRootPath).GetDirectories("Configuration").First();

		}


		public ChannelConfiguration GetConfigurationForChannel(string channelName)
		{

			if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentNullException(nameof(channelName));

			var configFile = _StorageFolder.GetFiles($"{channelName.ToLowerInvariant()}.json").FirstOrDefault();

			if (configFile == null || !configFile.Exists)
			{
				return new ChannelConfiguration()
				{
					ChannelName = channelName
				};
			}

			return JsonConvert.DeserializeObject<ChannelConfiguration>(File.ReadAllText(configFile.FullName), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});

		}

		public IEnumerable<string> GetConnectedChannels()
		{

			var configFileInfos = _StorageFolder.GetFiles($"*.json");
			if (configFileInfos == null || configFileInfos.Length == 0) return Array.Empty<string>();

			var outList = new List<string>();
			foreach (var configFile in configFileInfos)
			{
				try {
					var theConfig = JsonConvert.DeserializeObject<ChannelConfiguration>(File.ReadAllText(configFile.FullName), new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.All
					});
					if (theConfig.ConnectedToChannel) outList.Add(theConfig.ChannelName);
				} catch {
					// do nothing... ignore the deserialization issue
				}
			}

			return outList;

		}

		public void SaveConfigurationForChannel(string channelName, ChannelConfiguration config)
		{

			if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentNullException(nameof(channelName));
			if (config == null) throw new ArgumentNullException(nameof(config));

			var targetFile = new FileInfo(Path.Combine(_StorageFolder.FullName, $"{channelName.ToLowerInvariant()}.json"));
			File.WriteAllText(targetFile.FullName, JsonConvert.SerializeObject(config, Formatting.Indented,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				}));

		}
	}
}
