using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Functions
{
    public class BlobTriggerWebJob
    {
        readonly string _cosmosApi = Environment.GetEnvironmentVariable("CosmosApi");
        readonly HttpClient _client;

        public BlobTriggerWebJob(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [FunctionName("blobTriggerWebJob")]
        public async Task Run(
            [BlobTrigger("blober/{name}", Connection = "StorageConnectionString")] Stream myBlob,
            string name,
            ILogger logger)
        {
            logger.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            try
            {
                var jsonRequest = await new StreamReader(myBlob).ReadToEndAsync();
                var jsonContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var uriBuilder = new UriBuilder($"{_cosmosApi}/api/Candidate/Create/Collection");
                var response = await _client.PostAsync(uriBuilder.Uri, jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                logger.LogInformation($"StatusCode: '{response.StatusCode}'\nContent: '{content}'");
            }
            catch (Exception ex)
            {
                logger.LogError($"JSON content error:\n {ex.Message}");
            }
        }
    }
}
