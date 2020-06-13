using Cosmos.Common;
using Cosmos.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cosmos.Repository
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static string EndpointUri;
        private static string AccountKey;
        private static string DatabaseId;
        private static string CollectionId;
        private static string PartitionKeyPath;
        private static int MaxItems;
        private static CosmosClient client;
        private static Container container;

        public static async void Initialize(IConfiguration config)
        {
            EndpointUri = config["EndpointUri"];
            AccountKey = config["AccountKey"];
            DatabaseId = config["DatabaseId"];
            CollectionId = config["CollectionId"];
            PartitionKeyPath = config["PartitionKeyPath"];
            MaxItems = Int32.Parse(config["MaxItems"]);

            try
            {
                client = new CosmosClient(EndpointUri, AccountKey, new CosmosClientOptions {
                    SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
                });
                
                var dbResponse = await client.CreateDatabaseIfNotExistsAsync(DatabaseId);
                var colResponse  = await dbResponse.Database.CreateContainerIfNotExistsAsync(CollectionId, PartitionKeyPath);

                container = colResponse.Container;

                // If it's a new collection
                if (colResponse.StatusCode == HttpStatusCode.Created)
                {
                    // Mockup some data
                    await AddRangeAsync(BogusUtil.Candidates(MaxItems));
                }
            }

            catch (CosmosException ex)
            {
                throw new Exception($"Error occurred: {ex.StatusCode}", ex);
            }

            catch (Exception ex)
            {
                throw new Exception("Can't establish connection!", ex);
            }
        }

        public static Task<IQueryable<T>> GetAllAsync()
        {
            var items = container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true);
            return Task.FromResult<IQueryable<T>>(items);
        }

        public static async Task<Candidate> GetAsync(string id)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}'";
            var queryDefinition = new QueryDefinition(sqlQueryText);
            var iterator = container.GetItemQueryIterator<Candidate>(queryDefinition);

            if (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync();
                return currentResultSet.FirstOrDefault();
            }

            return null;
        }

        public static async Task<Candidate> AddAsync(Candidate candidate)
        {
            return await container.CreateItemAsync(candidate, new PartitionKey(candidate.LastName));
        }

        public static async Task<IEnumerable<Candidate>> AddRangeAsync(IEnumerable<Candidate> candidates)
        {
            var tasks = new List<Task>();
            foreach (var candidate in candidates)
            {
                tasks.Add(container.CreateItemAsync<Candidate>(candidate, new PartitionKey(candidate.LastName))
                    .ContinueWith(responseTask =>
                    {
                        if (responseTask.IsFaulted) 
                            throw responseTask.Exception;
                    }));
            }

            await Task.WhenAll(tasks);
            return candidates;
        }

        public static async Task<Candidate> UpdateAsync(Candidate candidate)
        {
            return await container.UpsertItemAsync(candidate);
        }

        public static async Task<Candidate> DeleteAsync(string id, string partitionKey)
        {
            return await container.DeleteItemAsync<Candidate>(id, new PartitionKey(partitionKey));
        }
    }
}
