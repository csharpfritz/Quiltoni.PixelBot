using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.Orchestrator.Data
{
	public class AzureChannelConfigurationContext : BaseAzureTableRepository, IChannelConfigurationContext
	{

		public override string TableName => nameof(ChannelConfiguration);

		public AzureChannelConfigurationContext(IConfiguration configuration)
		{
			base.Connectionstring = configuration["ChannelConfiguration:Connectionstring"];
		}

		public ChannelConfiguration GetConfigurationForChannel(string channelName)
		{

			var entity = base.GetEntityFromTable<ChannelConfigurationEntity>(CalculatePartitionKey(channelName), channelName).GetAwaiter().GetResult();
			return entity.Configuration;
		}

		public IEnumerable<string> GetConnectedChannels()
		{
			var records = base.GetRecordsFromTable<ChannelConfigurationEntity>(TableQuery.GenerateFilterCondition("Connected", QueryComparisons.Equal, "true")).GetAwaiter().GetResult();
			return records.Select(c => c.Configuration.ChannelName);
		}

		public void SaveConfigurationForChannel(string channelName, ChannelConfiguration config)
		{

			var entity = new ChannelConfigurationEntity(channelName)
			{
				Configuration = config
			};
			base.Save(entity).GetAwaiter().GetResult();

		}

		public static string CalculatePartitionKey(string channelName)
		{
			return channelName.Substring(0, 1);
		}

		public class ChannelConfigurationEntity : TableEntity
		{
			private ChannelConfiguration _Configuration;
			private bool _Connected;

			public ChannelConfigurationEntity() { }

			public ChannelConfigurationEntity(string channelName)
			{
				this.PartitionKey = CalculatePartitionKey(channelName);
				this.RowKey = channelName;
			}

			public ChannelConfiguration Configuration
			{
				get { return _Configuration; }
				set
				{
					Connected = value.ConnectedToChannel;
					_Configuration = value;
				}
			}

			public bool Connected { get; set; }

		}

	}
}
