using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cosmos.FunctionsApp
{
    public class CosmosWebJob
    {
        private readonly ILogger _logger;

        public CosmosWebJob(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CosmosWebJob>();
        }

        [Function("createCandidate")]
        [CosmosDBOutput(
            Constants.DatabaseName,
            Constants.ContainerName,
            Connection = Constants.CosmosConnectionString)]
        public async Task<object> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "candidate")] HttpRequest req,
            [CosmosDBInput(
                databaseName: Constants.DatabaseName,
                containerName: Constants.ContainerName,
                Connection = Constants.CosmosConnectionString)]
            List<Candidate> candidates)
        {
            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var candidate = JsonConvert.DeserializeObject<Candidate>(request)!;

            if (candidates == null || candidates.Any(o => o.Id == candidate.Id)) return null!;

            _logger.LogInformation("Create:{FullName}, ID:{Id}", candidate.FullName, candidate.Id);

            // Cosmos Output
            candidates.Add(new Candidate { Id = candidate.Id });

            return candidates
                .Where(o => o.Id == candidate.Id)
                .Select(o => new
                {
                    id = o.Id,
                    lastName = candidate.LastName,
                    firstName = candidate.FirstName,
                    email = candidate.Email,
                    balance = candidate.Balance,
                    points = candidate.Points,
                    registrationDate = candidate.RegistrationDate,
                    isActive = candidate.IsActive,
                    technologies = candidate.Technologies
                });
        }

        [Function("updateCandidate")]
        [CosmosDBOutput(
            Constants.DatabaseName,
            Constants.ContainerName,
            Connection = Constants.CosmosConnectionString)]
        public async Task<object> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "candidate/{id}")] HttpRequest req, Guid id,
            [CosmosDBInput(
                databaseName: Constants.DatabaseName,
                containerName: Constants.ContainerName,
                Connection = Constants.CosmosConnectionString)]
            IList<Candidate> candidates)
        {
            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var candidate = JsonConvert.DeserializeObject<Candidate>(request)!;

            if (candidates == null || !candidates.Any(o => o.Id == id.ToString())) return null!;

            _logger.LogInformation("Update:{FullName}, ID:{id}", candidate.FullName, id);

            // Cosmos Output
            return candidates
                .Where(o => o.Id == id.ToString())
                .Select(o => new
                {
                    id = o.Id,
                    lastName = candidate.LastName,
                    firstName = candidate.FirstName,
                    email = candidate.Email,
                    balance = candidate.Balance,
                    points = candidate.Points,
                    registrationDate = candidate.RegistrationDate,
                    isActive = candidate.IsActive,
                    technologies = candidate.Technologies
                });
        }
    }
}
