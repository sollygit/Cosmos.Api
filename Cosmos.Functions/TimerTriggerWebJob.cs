using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cosmos.Functions
{
    public class TimerTriggerWebJob
    {
        readonly string _cosmosApi = Environment.GetEnvironmentVariable("CosmosApi");
        readonly HttpClient _client;

        public TimerTriggerWebJob(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [FunctionName("transferBoardData")]
        public async Task TransferBoardData(
            [TimerTrigger("*/30 * * * * *")] TimerInfo myTimer,
            ILogger logger)
        {
            if (myTimer == null) await Task.FromResult(HttpStatusCode.BadRequest);

            logger.LogInformation($"transferBoardData on {DateTime.Now}");
            var response = await _client.GetAsync($"{_cosmosApi}/api/board"); // Invoke 'transferBoardData'
            logger.LogInformation($"SignalR Board Response: {response.StatusCode}");

            await Task.FromResult(response.StatusCode);
        }
    }
}
