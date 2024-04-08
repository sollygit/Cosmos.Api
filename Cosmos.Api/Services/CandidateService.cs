using Cosmos.Common;
using Cosmos.Model;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Api.Services
{
    public interface ICandidateService
    {
        Task<IEnumerable<Candidate>> GetAsync();
        Task<Candidate> GetAsync(string id);
        Task<Candidate> CreateAsync(Candidate candidate);
        Task<IEnumerable<Candidate>> CreateAsync(IEnumerable<Candidate> candidates);
        Task<IEnumerable<Candidate>> CreateAsync(int count, bool saveToDatabase = false);
        Task<Candidate> CreateDummyAsync();
        Task<Candidate> UpdateAsync(string id, string partitionKey, Candidate candidate);
        Task<Candidate> DeleteAsync(string id);
    }

    public class CandidateService : ICandidateService
    {
        readonly IRepository<Candidate> _candidateRepository;

        public CandidateService( IRepository<Candidate> candidateRepository) =>
            (_candidateRepository) = (candidateRepository);

        public async Task<IEnumerable<Candidate>> GetAsync()
        {
            return await _candidateRepository.GetAsync(o => o.RegistrationDate > new DateTime(2020, 1, 1));
        }

        public async Task<Candidate> GetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Candidate ID cannot be empty");
            }

            return await _candidateRepository.GetAsync(id);
        }

        public async Task<Candidate> CreateAsync(Candidate candidate)
        {
            if (candidate == null)
            {
                throw new Exception("Candidate cannot be null");
            }

            return await _candidateRepository.CreateAsync(candidate);
        }

        public async Task<IEnumerable<Candidate>> CreateAsync(IEnumerable<Candidate> candidates)
        {
            if (candidates == null)
            {
                throw new Exception("Candidates cannot be null");
            }

            return await _candidateRepository.CreateAsync(candidates);
        }

        public async Task<IEnumerable<Candidate>> CreateAsync(int count, bool saveToDB)
        {
            var items = BogusUtil.Candidates(count);
            if (saveToDB)
            {
                await _candidateRepository.CreateAsync(items);
            }
            return items;
        }

        public async Task<Candidate> CreateDummyAsync()
        {
            var items = BogusUtil.Candidates(1);
            var dummy = items.FirstOrDefault();
            dummy.IsActive = false;
            dummy.Technologies = null;
            return await _candidateRepository.CreateAsync(dummy);
        }

        public async Task<Candidate> UpdateAsync(string id, string partitionKey, Candidate candidate)
        {
            var item = await _candidateRepository.GetAsync(id);

            item.FirstName = candidate.FirstName;
            item.LastName = candidate.LastName;
            item.Email = candidate.Email;
            item.Balance = candidate.Balance;
            item.Points = candidate.Points;
            item.RegistrationDate = candidate.RegistrationDate;
            item.IsActive = candidate.IsActive;
            item.Technologies = candidate.Technologies;

            return await _candidateRepository.UpdateAsync(item);
        }

        public async Task<Candidate> DeleteAsync(string id)
        {
            var item = await _candidateRepository.GetAsync(id);
            await _candidateRepository.DeleteAsync(id);

            return item;
        }
    }
}
