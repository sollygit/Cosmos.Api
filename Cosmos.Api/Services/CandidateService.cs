using Cosmos.Api.Interfaces;
using Cosmos.Common;
using Cosmos.Model;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Api.Services
{
    public class CandidateService : ICandidateService
    {
        readonly IRepository<Candidate> _candidateRepository;

        public CandidateService(
            IRepository<Candidate> candidateRepository) =>
            (_candidateRepository) = (candidateRepository);

        public async ValueTask<IEnumerable<Candidate>> GetAll()
        {
            return await _candidateRepository.GetAsync(o => o.RegistrationDate > new DateTime(2020, 1, 1));
        }

        public async ValueTask<Candidate> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Candidate ID cannot be empty");
            }

            return await _candidateRepository.GetAsync(id);
        }

        public async ValueTask<Candidate> Add(Candidate candidate)
        {
            if (candidate == null)
            {
                throw new Exception("Candidate cannot be null");
            }

            return await _candidateRepository.CreateAsync(candidate);
        }

        public async ValueTask<Candidate> Update(string id, string partitionKey, Candidate candidate)
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

        public async ValueTask<Candidate> Delete(string id)
        {
            var item = await _candidateRepository.GetAsync(id);
            await _candidateRepository.DeleteAsync(id);

            return item;
        }

        public async ValueTask<IEnumerable<Candidate>> Generate(int count)
        {
            var items = BogusUtil.Candidates(count);

            foreach (var item in items)
            {
                await _candidateRepository.CreateAsync(item);
            }

            return items;
        }
    }
}
