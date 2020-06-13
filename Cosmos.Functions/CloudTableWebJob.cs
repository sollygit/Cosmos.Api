using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Cosmos.Functions
{
    public class CloudTableWebJob
    {
        private readonly string _storageConnection = Environment.GetEnvironmentVariable("StorageConnectionString");
        private readonly CosmosDBHelper _cosmosDBHelper;
        private readonly CloudTable _table;

        public CloudTableWebJob()
        {
            _cosmosDBHelper = new CosmosDBHelper(_storageConnection);
            _table = AsyncHelper.RunAsync(async () =>
            {
                return await _cosmosDBHelper.CreateTableAsync("demo");
            });
        }

        [FunctionName("negotiate")]
        public IActionResult GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "board")] SignalRConnectionInfo connectionInfo,
            ILogger logger)
        {
            if (req == null) return null;

            logger.LogInformation("Negotiate trigger function processed a request.");
            return connectionInfo != null ? 
                (ActionResult)new OkObjectResult(connectionInfo) :
                new NotFoundObjectResult("SignalR could not load");
        }

        [FunctionName("sendNotice")]
        public async Task<DataEntity> SendNotice(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post")] object notice,
           [SignalR(HubName = "board")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var entityCount = new DataEntity("demo", "NoticeCount") { IntValue = 0 };
            var data = await _cosmosDBHelper.RetrieveEntityUsingPointQueryAsync(_table, "demo", "NoticeCount");
            if (data != null)
            {
                entityCount.IntValue = data.IntValue + 1;
            }

            var noticeText = JObject.Parse(notice.ToString()).Value<string>("noticeText");
            var entityText = new DataEntity("demo", "NoticeText") { StringValue = noticeText };

            await _cosmosDBHelper.InsertOrMergeEntityAsync(_table, entityCount);
            await _cosmosDBHelper.InsertOrMergeEntityAsync(_table, entityText);
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "newNotice",
                Arguments = new[] { notice }
            });

            return entityText;
        }

        [FunctionName("getNotice")]
        public async Task<DataEntity> GetNotice(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalR(HubName = "board")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (req == null || signalRMessages == null) return await Task.FromResult<DataEntity>(null);

            var audienceCount = new DataEntity("demo", "AudienceCount") { IntValue = 0 };
            var data = await _cosmosDBHelper.RetrieveEntityUsingPointQueryAsync(_table, "demo", "AudienceCount");
            if (data != null)
            {
                audienceCount.IntValue = data.IntValue + 1;
                audienceCount.Timestamp = data.Timestamp;
            }

            return await _cosmosDBHelper.InsertOrMergeEntityAsync(_table, audienceCount);
        }

        [FunctionName("chart")]
        public async Task<IActionResult> GetChartData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            ILogger log)
        {
            if (req == null) return await Task.FromResult(new BadRequestObjectResult(""));

            log.LogInformation($"TransferChartData Timer trigger {DateTime.Now}");
            var chartData = await GetChartData();
            return new OkObjectResult(chartData);
        }

        [FunctionName("transferChartData")]
        public async Task Run(
            [TimerTrigger("*/30 * * * * *")] TimerInfo myTimer,
            [SignalR(HubName = "board")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            if (myTimer == null) return;

            log.LogInformation($"transferChartData on {DateTime.Now}");

            var chartData = await GetChartData();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "transferChartData",
                Arguments = new[] { chartData }
            });
        }

        private async Task<Chart[]> GetChartData()
        {
            var commentCount = await _cosmosDBHelper.RetrieveEntityUsingPointQueryAsync(_table, "demo", "CommentCount");
            if (commentCount == null)
            {
                commentCount = new DataEntity("demo", "CommentCount") { IntValue = 0 };
            }

            var noticeCount = await _cosmosDBHelper.RetrieveEntityUsingPointQueryAsync(_table, "demo", "NoticeCount");
            if (noticeCount == null)
            {
                noticeCount = new DataEntity("demo", "NoticeCount") { IntValue = 0 };
            }

            var audienceCount = await _cosmosDBHelper.RetrieveEntityUsingPointQueryAsync(_table, "demo", "AudienceCount");
            if (audienceCount == null)
            {
                audienceCount = new DataEntity("demo", "AudienceCount") { StringValue = "" };
            }

            return new Chart[] {
                new Chart { Data = new int[] { commentCount.IntValue }, Label = "Comments" },
                new Chart { Data = new int[] { noticeCount.IntValue }, Label = "Notices" },
                new Chart { Data = new int[] { audienceCount.IntValue }, Label = "Users" },
            };
        }
    }
}
