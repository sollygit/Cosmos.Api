using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cosmos.Functions
{
    public class ChangeFeedWebJob
    {
        readonly string _cosmosApi = Environment.GetEnvironmentVariable("CosmosApi");
        readonly HttpClient _client;

        public ChangeFeedWebJob(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [FunctionName("changeFeedListener")]
        public async Task ChangeFeedListener([CosmosDBTrigger(
            databaseName: Constants.DatabaseName,
            collectionName: Constants.CollectionName,
            ConnectionStringSetting = Constants.CosmosConnectionString,
            LeaseCollectionName = Constants.LeaseCollectionName,
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input == null || input.Count == 0) await Task.FromResult(HttpStatusCode.BadRequest);

            log.LogInformation($"Documents modified {input.Count}. First document Id {input[0].Id}");

            var candidateResponse = await _client.GetAsync($"{_cosmosApi}/api/candidate"); // Invoke 'sendCandidates'
            var chartResponse = await _client.GetAsync($"{_cosmosApi}/api/chart"); // Invoke 'transferChartData'

            log.LogInformation($"SignalR Candidate Response: {candidateResponse.StatusCode}");
            log.LogInformation($"SignalR Chart Response: {chartResponse.StatusCode}");

            await Task.FromResult(candidateResponse.StatusCode);
        }
    }
}
