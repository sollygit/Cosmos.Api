using Cosmos.Api.HubConfig;
using Cosmos.Api.Interfaces;
using Cosmos.Common;
using Cosmos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
            var candidates = await _candidateService.GetAll();
            await _hub.Clients.All.SendAsync("sendCandidates", candidates);
            return Ok(new { Message = "sendCandidates success" });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> All()
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
        public async Task<IActionResult> Add([FromBody] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _candidateService.Add(candidate);

            return new ObjectResult(result);
        }

        [HttpPost("[action]/{count}")]
        public async Task<IActionResult> Generate(int count)
        {
            var result = await _candidateService.Generate(count);
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

            var original = await _candidateService.Get(id);

            if (original == null)
            {
                return NotFound(id);
            }

            var result = await _candidateService.Update(candidate.Id, original.LastName, candidate);

            return new ObjectResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _candidateService.Delete(id);

            return Ok(result);
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
