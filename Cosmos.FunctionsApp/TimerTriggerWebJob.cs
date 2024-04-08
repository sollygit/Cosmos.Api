using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Cosmos.FunctionsApp
{
    public class TimerTriggerWebJob
    {
        readonly string _cosmosApi = Environment.GetEnvironmentVariable("CosmosApi")!;
        readonly ILogger<TimerTriggerWebJob> _logger;
        readonly HttpClient _client;

        public TimerTriggerWebJob(ILogger<TimerTriggerWebJob> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient();
        }

        [Function("timerTriggerBoardData")]
        public async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            _logger.LogInformation("timerTriggerBoardData on {Date}", DateTime.Now);
            var response = await _client.GetAsync($"{_cosmosApi}/api/board"); // Invoke timerTriggerBoardData
            _logger.LogInformation("SignalR Board Response: {StatusCode}", response.StatusCode);

            await Task.FromResult(response.StatusCode);
        }
    }
}
