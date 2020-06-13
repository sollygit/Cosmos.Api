using Cosmos.Common;
using Cosmos.Interface;
using Cosmos.Model;
using Cosmos.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Api.Services
{
    public class CandidateService : ICandidateService
    {
        public async Task<IQueryable<Candidate>> GetAll()
        {
            return await DocumentDBRepository<Candidate>.GetAllAsync();
        }

        public async Task<Candidate> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Candidate ID cannot be empty");
            }

            return await DocumentDBRepository<Candidate>.GetAsync(id);
        }

        public async Task<Candidate> Add(Candidate candidate)
        {
            if (candidate == null)
            {
                throw new Exception("Candidate cannot be null");
            }

            return await DocumentDBRepository<Candidate>.AddAsync(candidate);
        }

        public async Task<Candidate> Update(string id, string partitionKey, Candidate candidate)
        {
            if (partitionKey.ToLower() != candidate.LastName.ToLower())
            {
                await Delete(id, partitionKey);
            }

            return await DocumentDBRepository<Candidate>.UpdateAsync(candidate);
        }

        public async Task<Candidate> Delete(string id, string partitionKey)
        {
            return await DocumentDBRepository<Candidate>.DeleteAsync(id, partitionKey);
        }

        public async Task<IEnumerable<Candidate>> Generate(int count)
        {
            var items = BogusUtil.Candidates(count);
            return await DocumentDBRepository<Candidate>.AddRangeAsync(items);
        }
    }
}
