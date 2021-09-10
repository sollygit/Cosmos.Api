using Cosmos.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Api.Interfaces
{
    public interface ICandidateService
    {
        ValueTask<IEnumerable<Candidate>> GetAll();
        ValueTask<Candidate> Get(string id);
        ValueTask<Candidate> Add(Candidate candidate);
        ValueTask<Candidate> Update(string id, string partitionKey, Candidate candidate);
        ValueTask<IEnumerable<Candidate>> Generate(int count);
        ValueTask<Candidate> Delete(string id);
    }
}
