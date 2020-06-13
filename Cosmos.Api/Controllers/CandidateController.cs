using Cosmos.Api.HubConfig;
using Cosmos.Interface;
using Cosmos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly IHubContext<CandidateHub> _hub;
        readonly ICandidateService _candidateService;

        public CandidateController(IHubContext<CandidateHub> hub, ICandidateService candidateService)
        {
            _hub = hub;
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await _hub.Clients.All.SendAsync("sendCandidates", await _candidateService.GetAll());
            return Ok(new { Message = "Candidates Request Completed" });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var candidates = await _candidateService.GetAll();
            return new ObjectResult(candidates);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var candidate = await _candidateService.Get(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return new ObjectResult(candidate);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add([FromBody] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _candidateService.Get(candidate.Id);

            if (item != null)
            {
                return BadRequest($"Item with ID '{candidate.Id}' already exists.");
            }

            var result = await _candidateService.Add(candidate);

            return new ObjectResult(result);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Candidate candidate)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != candidate.Id)
            {
                return BadRequest(ModelState);
            }

            var original = await _candidateService.Get(id);

            if (original == null)
            {
                return NotFound(id);
            }

            var result = await _candidateService.Update(candidate.Id, original.LastName, candidate);

            return new ObjectResult(result);
        }

        [HttpPost("[action]/{count}")]
        public async Task<IActionResult> Generate(int count)
        {
            var result = await _candidateService.Generate(count);
            return new ObjectResult(result);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var candidate = await _candidateService.Get(id);

            if (candidate == null)
            {
                return NotFound(id);
            }

            await _candidateService.Delete(id, candidate.LastName);

            return Ok(candidate);
        }
    }
}
