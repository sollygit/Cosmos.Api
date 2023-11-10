using Cosmos.Api.Configurations;
using Cosmos.Api.Models;
using Cosmos.Common;
using Cosmos.Model;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Api.Services
{
    public interface ICloudTableService
    {
        Task<bool> CreateIfNotExistsAsync(string tableName);
        Task<DataEntity> RetrieveAsync(string partitionKey, string rowKey);
        Task<DataEntity> InsertOrMergeAsync(DataEntity entity);
        Task<DataEntity> GetNotice();
        Task<DataEntity> CreateNotice(DataEntity dataEntity);
        Task<IEnumerable<Chart>> GetBoardData();
    }

    public class CloudTableService : ICloudTableService
    {
        readonly StorageConfig _config;
        const string TABLE_NAME = "board";
        const string PARTITION_KEY = "BOARD";
        CloudTable table;

        public CloudTableService(IOptions<StorageConfig> config)
        {
            _config = config.Value;
            AsyncHelper.RunAsync(async () => {
                await CreateIfNotExistsAsync(TABLE_NAME);
            });
        }

        public async Task<bool> CreateIfNotExistsAsync(string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(_config.ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            table = tableClient.GetTableReference(tableName);
            return await table.CreateIfNotExistsAsync();
        }

        public async Task<DataEntity> RetrieveAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<DataEntity>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);
            return result.Result as DataEntity;
        }

        public async Task<DataEntity> InsertOrMergeAsync(DataEntity entity)
        {
            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            var result = await table.ExecuteAsync(insertOrMergeOperation);
            return result.Result as DataEntity;
        }

        public async Task<DataEntity> GetNotice()
        {
            var audienceCount = new DataEntity(PARTITION_KEY, "AudienceCount") { IntValue = 0 };
            var data = await RetrieveAsync(PARTITION_KEY, "AudienceCount");
            if (data != null)
            {
                audienceCount.IntValue = data.IntValue + 1;
                audienceCount.Timestamp = data.Timestamp;
            }
            return await InsertOrMergeAsync(audienceCount);
        }

        public async Task<DataEntity> CreateNotice(DataEntity dataEntity)
        {
            if (dataEntity == null) return await Task.FromResult<DataEntity>(null);

            var commentCount = new DataEntity(PARTITION_KEY, "CommentCount") { IntValue = 0 };
            var noticeCount = new DataEntity(PARTITION_KEY, "NoticeCount") { IntValue = 0 };
            var data = await RetrieveAsync(PARTITION_KEY, "NoticeCount");
            if (data != null)
            {
                noticeCount.IntValue = data.IntValue + 1;
            }
            var noticeText = new DataEntity(PARTITION_KEY, "NoticeText") { StringValue = dataEntity.StringValue };
            await InsertOrMergeAsync(commentCount);
            await InsertOrMergeAsync(noticeCount);
            await InsertOrMergeAsync(noticeText);

            return noticeText;
        }

        public async Task<IEnumerable<Chart>> GetBoardData()
        {
            var commentCount = await RetrieveAsync(PARTITION_KEY, "CommentCount");
            if (commentCount == null)
            {
                commentCount = new DataEntity(PARTITION_KEY, "CommentCount") { IntValue = 0 };
            }

            var noticeCount = await RetrieveAsync(PARTITION_KEY, "NoticeCount");
            if (noticeCount == null)
            {
                noticeCount = new DataEntity(PARTITION_KEY, "NoticeCount") { IntValue = 0 };
            }

            var audienceCount = await RetrieveAsync(PARTITION_KEY, "AudienceCount");
            if (audienceCount == null)
            {
                audienceCount = new DataEntity(PARTITION_KEY, "AudienceCount") { StringValue = "" };
            }

            return new Chart[] {
                new Chart { Data = new int[] { commentCount.IntValue }, Label = "Comments" },
                new Chart { Data = new int[] { noticeCount.IntValue }, Label = "Notices" },
                new Chart { Data = new int[] { audienceCount.IntValue }, Label = "Users" },
            };
        }
    }
}
