using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quiltoni.PixelBot.Core.Data;

namespace PixelBot.Orchestrator.Data
{

	public class AzureWidgetStateRepository : BaseAzureTableRepository, IWidgetStateRepository
	{

		// cheer cpayette 775 24/10/2019
		// cheer mholloway 550 24/10/2019        

		public AzureWidgetStateRepository(IConfiguration configuration)
		{
			this.Connectionstring = configuration["WidgetPersistence:Connectionstring"];
		}

		public override string TableName => "WidgetState";

		public async Task<Dictionary<string, string>> Get(string channelName, string widgetName)
		{

			var entity = await base.GetEntityFromTable<WidgetStateEntity>(channelName, widgetName);
			return entity.GetPayloadAsDictionary();

		}

		public async Task Save(string channelName, string widgetName, Dictionary<string, string> payload)
		{

			var entity = new WidgetStateEntity(channelName, widgetName);
			entity.SetPayloadFromDictionary(payload);

			await base.Save(entity);

		}

		public class WidgetStateEntity : TableEntity
		{

			public WidgetStateEntity() { }

			public WidgetStateEntity(string channelName, string widgetName)
			{

				PartitionKey = channelName;
				RowKey = widgetName;

			}

			public string Payload { get; set; }

			public Dictionary<string, string> GetPayloadAsDictionary()
			{

				return JsonConvert.DeserializeObject<Dictionary<string, string>>(Payload);

			}

			public void SetPayloadFromDictionary(Dictionary<string, string> newPayload)
			{

				Payload = JsonConvert.SerializeObject(newPayload);

			}

		}

	}


}