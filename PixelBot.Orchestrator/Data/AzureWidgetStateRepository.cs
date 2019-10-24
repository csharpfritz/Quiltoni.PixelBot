using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quiltoni.PixelBot.Core.Data;

namespace PixelBot.Orchestrator.Data {

    public class AzureWidgetStateRepository : IWidgetStateRepository
    {

        // cheer cpayette 775 24/10/2019
        // cheer mholloway 550 24/10/2019        

        public AzureWidgetStateRepository(IConfiguration configuration) {
            this.Connectionstring = configuration["WidgetPersistenceConnectionstring"];
        }

        public string Connectionstring { get; }

        public async Task<Dictionary<string, string>> Get(string channelName, string widgetName)
        {

            var table = await GetAzureTable();

            var retrieveOperation = TableOperation.Retrieve<WidgetStateEntity>(channelName, widgetName);
            var result = await table.ExecuteAsync(retrieveOperation);
            return (result.Result as WidgetStateEntity).GetPayloadAsDictionary();

        }

        public async Task Save(string channelName, string widgetName, Dictionary<string, string> payload)
        {
            
            var entity = new WidgetStateEntity(channelName, widgetName);
            entity.SetPayloadFromDictionary(payload);

            var table = await GetAzureTable();
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            var result = await table.ExecuteAsync(insertOrMergeOperation);

        }

        private async Task<CloudTable> GetAzureTable()
        {
            var account = CloudStorageAccount.Parse(Connectionstring);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WidgetState");
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public class WidgetStateEntity : TableEntity {
            
            public WidgetStateEntity(string channelName, string widgetName)
            {
                
                PartitionKey = channelName;
                RowKey = widgetName;

            }

            public string Payload { get; set; }

            public Dictionary<string,string> GetPayloadAsDictionary() {

                return JsonConvert.DeserializeObject<Dictionary<string,string>>(Payload);

            }

            public void SetPayloadFromDictionary(Dictionary<string,string> newPayload) {

                Payload = JsonConvert.SerializeObject(newPayload);

            }

        }

    }


}