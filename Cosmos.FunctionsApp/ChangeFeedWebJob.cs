using Cosmos.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Cosmos.FunctionsApp
{
    public class ChangeFeedWebJob
    {
        readonly string _cosmosApi = Environment.GetEnvironmentVariable("CosmosApi")!;
        ILogger _logger;
        readonly HttpClient _client;

        public ChangeFeedWebJob(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<ChangeFeedWebJob>();
            _client = httpClientFactory.CreateClient();
        }

        [Function("ChangeFeedWebJob")]
        public async Task Run([CosmosDBTrigger(
            databaseName: Constants.DatabaseName,
            containerName: Constants.ContainerName,
            Connection = Constants.CosmosConnectionString,
            LeaseContainerName = Constants.LeaseContainerName,
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Candidate> candidates)
        {
            if (candidates != null && candidates.Count > 0)
            {
                _logger.LogInformation($"Candidate {candidates[0].FullName} has been modified. Documents Count:{candidates.Count}.");

                var candidateResponse = await _client.GetAsync($"{_cosmosApi}/api/candidate"); // Invoke sendCandidates
                var chartResponse = await _client.GetAsync($"{_cosmosApi}/api/chart"); // Invoke transferChartData

                _logger.LogInformation($"SignalR Candidate Response: {candidateResponse.StatusCode}");
                _logger.LogInformation($"SignalR Chart Response: {chartResponse.StatusCode}");

                await Task.FromResult(candidateResponse.StatusCode);
            }
        }
    }
}
