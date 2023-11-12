using Cosmos.Api.HubConfig;
using Cosmos.Api.Services;
using Cosmos.Common;
using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly IHubContext<CandidateHub> _hub;
        readonly ICandidateService _candidateService;

        public CandidateController(ILogger<CandidateController> logger, IHubContext<CandidateHub> hub, ICandidateService candidateService)
        {
            _logger = logger;
            _hub = hub;
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<IActionResult> SendCandidates()
        {
            var candidates = await _candidateService.GetAsync();
            await _hub.Clients.All.SendAsync("sendCandidates", candidates);
            return Ok(new { Message = "sendCandidates success" });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> All()
        {
            var candidates = await _candidateService.GetAsync();
            return new ObjectResult(candidates);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var candidate = await _candidateService.GetAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return new ObjectResult(candidate);
        }

        [HttpGet("{residentialType}")]
        public ActionResult Get(ResidentialType residentialType = ResidentialType.InState)
        {
            _logger.LogInformation($"query {residentialType} candidates");
            if (residentialType == ResidentialType.International)
            {
                _logger.LogInformation("found 10000 candidates.");
            }
            return Ok();
        }

        [HttpGet("{id:int}/Forms/{formId:int}")]
        [ProducesResponseType(typeof(FormSubmissionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<FormSubmissionResult> ViewForm(int id, int formId)
        {
            _logger.LogInformation($"viewing the form#{formId} for Candidate ID={id}");
            await Task.Delay(1000);
            return new FormSubmissionResult { FormId = formId, CandidateId = id };
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.CreateAsync(candidate);

            return new ObjectResult(result);
        }

        [HttpPost("[action]/Collection")]
        public async Task<IActionResult> Create([FromBody] IEnumerable<Candidate> candidates)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.CreateAsync(candidates);

            return new ObjectResult(result);
        }

        [HttpPost("[action]/{count}/{saveToDatabase}")]
        public async Task<IActionResult> Generate(int count, bool saveToDatabase = false)
        {
            var result = await _candidateService.CreateAsync(count, saveToDatabase);
            return new ObjectResult(result);
        }

        [HttpPost("{id:int}/Forms")]
        [ProducesResponseType(typeof(FormSubmissionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FormSubmissionResult>> SubmitForm(int id, [FromForm] CandidateForm form)
        {
            _logger.LogInformation($"validating the form#{form.FormId} for Candidate ID={id}");
            _logger.LogInformation($"saving file [{form.CandidateFile.FileName}]");
            await Task.Delay(1500);
            _logger.LogInformation("file saved.");
            var result = new FormSubmissionResult { FormId = form.FormId, CandidateId = id };
            return CreatedAtAction(nameof(ViewForm), new { id, form.FormId }, result);
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

            var original = await _candidateService.GetAsync(id);

            if (original == null)
            {
                return NotFound(id);
            }

            var result = await _candidateService.UpdateAsync(candidate.Id, original.LastName, candidate);

            return new ObjectResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _candidateService.DeleteAsync(id);

            return Ok(result);
        }

        [HttpDelete("All")]
        public async Task<IActionResult> Delete()
        {
            var candidates = await _candidateService.GetAsync();

            foreach (var candidate in candidates)
            {
                await _candidateService.DeleteAsync(candidate.Id);
            }

            // Insert a dummy candidate to allow for a change feed trigger
            await _candidateService.CreateDummyAsync();

            return Ok(new { Message = $"{candidates.Count()} candidates have been deleted" });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{id:int}/Forms/{formId:int}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<bool> Delete(int id, int formId)
        {
            _logger.LogInformation($"deleting the form#{formId} for candidate ID=[{id}]");
            await Task.Delay(1500);
            return true;
        }
    }
}
