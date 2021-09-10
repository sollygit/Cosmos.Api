using Microsoft.Azure.Cosmos.Table;

namespace Cosmos.Api.Models
{
    public class DataEntity : TableEntity
    {
        public DataEntity()
        {
        }

        public DataEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }
}
