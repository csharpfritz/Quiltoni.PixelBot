using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace PixelBot.Orchestrator.Data
{
	public abstract class BaseAzureTableRepository
	{

		public string Connectionstring { get; protected set; }
		public abstract string TableName { get; }

		protected async Task<CloudTable> GetAzureTable()
		{
			var account = CloudStorageAccount.Parse(Connectionstring);
			var tableClient = account.CreateCloudTableClient();
			var table = tableClient.GetTableReference(TableName);
			await table.CreateIfNotExistsAsync();
			return table;
		}

		protected async Task<T> GetEntityFromTable<T>(string partitionKey, string key) where T: TableEntity, new()
		{

			var table = await GetAzureTable();

			var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, key);
			var result = await table.ExecuteAsync(retrieveOperation);
			if (result.Result == null) return new T();
			return result.Result as T;

		}

		protected async Task<IEnumerable<T>> GetRecordsFromTable<T>(string tableFilter) where T:TableEntity, new()
		{

			var table = await GetAzureTable();
			var query = new TableQuery<T>()
				.Where(tableFilter);

			var result = await table.ExecuteQuerySegmentedAsync<T>(query, null);
			return result.Results;

		}



		protected async Task Save<T>(T payloadToSave) where T: TableEntity
		{

			var table = await GetAzureTable();
			var insertOrMergeOperation = TableOperation.InsertOrMerge(payloadToSave);
			var result = await table.ExecuteAsync(insertOrMergeOperation);

		}

	}


}