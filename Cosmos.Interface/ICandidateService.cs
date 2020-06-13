using Cosmos.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Interface
{
    public interface ICandidateService
    {
        Task<IQueryable<Candidate>> GetAll();
        Task<Candidate> Get(string id);
        Task<Candidate> Add(Candidate candidate);
        Task<Candidate> Update(string id, string partitionKey, Candidate candidate);
        Task<IEnumerable<Candidate>> Generate(int count);
        Task<Candidate> Delete(string id, string partitionKey);
    }
}
