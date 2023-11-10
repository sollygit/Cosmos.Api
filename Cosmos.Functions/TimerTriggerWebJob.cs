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

        [FunctionName("timerTriggerBoardData")]
        public async Task TimerTriggerBoardData(
            [TimerTrigger("*/30 * * * * *")] TimerInfo myTimer,
            ILogger logger)
        {
            if (myTimer == null) await Task.FromResult(HttpStatusCode.BadRequest);

            logger.LogInformation("timerTriggerBoardData on {Date}", DateTime.Now);
            var response = await _client.GetAsync($"{_cosmosApi}/api/board"); // Invoke 'timerTriggerBoardData'
            logger.LogInformation($"SignalR Board Response: {response.StatusCode}");

            await Task.FromResult(response.StatusCode);
        }
    }
}
