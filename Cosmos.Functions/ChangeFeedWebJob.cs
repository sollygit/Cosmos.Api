using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [FunctionName("createCandidate")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "candidate")] HttpRequest req,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.CollectionName,
                ConnectionStringSetting = Constants.CosmosConnectionString)]
            IAsyncCollector<Candidate> items, ILogger logger)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<Candidate>(request);

            logger.LogInformation($"Create new item:{item.FullName}, ID:{item.Id}");

            await items.AddAsync(item);

            return new OkObjectResult(item);
        }

        [FunctionName("updateCandidate")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "candidate/{id}")] HttpRequest req,
            string id,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.CollectionName,
                ConnectionStringSetting = Constants.CosmosConnectionString)] 
            DocumentClient client, ILogger logger)
        {
            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<Candidate>(request);
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var uri = UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.CollectionName);
            var document = client.CreateDocumentQuery(uri, option)
                .Where(t => t.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (document == null)
            {
                logger.LogError($"Item {id} not found. It may not exist!");
                return new NotFoundResult();
            }

            document.SetPropertyValue("firstName", item.FirstName);
            document.SetPropertyValue("lastName", item.LastName);
            document.SetPropertyValue("email", item.Email);
            document.SetPropertyValue("balance", item.Balance);
            document.SetPropertyValue("points", item.Points);
            document.SetPropertyValue("registrationDate", item.RegistrationDate);
            document.SetPropertyValue("technologies", item.Technologies);
            document.SetPropertyValue("isActive", item.IsActive);

            logger.LogInformation($"Update item:{item.FullName}, ID:{item.Id}");

            await client.ReplaceDocumentAsync(document);

            Candidate updated = (dynamic)document;

            return new OkObjectResult(updated);
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
