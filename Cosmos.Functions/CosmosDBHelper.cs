using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Cosmos.Functions
{
    public class CosmosDBHelper
    {
        private readonly string storageConnectionString;

        public CosmosDBHelper(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;
        }

        public async Task<CloudTable> CreateTableAsync(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task<DataEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DataEntity>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            DataEntity data = result.Result as DataEntity;
            return data;
        }

        public async Task<DataEntity> InsertOrMergeEntityAsync(CloudTable table, DataEntity entity)
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            DataEntity insertedData = result.Result as DataEntity;
            return insertedData;
        }
    }
}
