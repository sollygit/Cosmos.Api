using Cosmos.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlobController : ControllerBase
    {
        readonly ILogger<BlobController> _logger;
        readonly IStorageService _storageService;

        public BlobController(ILogger<BlobController> logger, IStorageService storageService) =>
            (_logger, _storageService) = (logger, storageService);

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var item = await _storageService.GetBlobItemAsync(name);
            if (item == null) return NotFound($"Item '{name}' not found");
            return Ok(item);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> All()
        {
            var items = await _storageService.GetBlobItemsAsync();
            return Ok(items);
        }

        [HttpPost("Single")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null) return BadRequest("A file is required!");
            _logger.LogInformation($"Upload file: '{file.FileName}'");
            var filename = await _storageService.UploadFromStreamAsync(file);
            return Ok(filename);
        }

        [HttpPost("Multiple")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            _logger.LogInformation($"validating {files.Count} files");
            foreach (var file in files)
            {
                _logger.LogInformation($"saving file {file.FileName}");
                await Task.Delay(1000);
            }
            _logger.LogInformation("All files saved.");
            return await Task.FromResult(new OkObjectResult("multiple-files upload success"));
        }

        [HttpDelete("{name}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string name)
        {
            _logger.LogInformation($"Deleting filename '{name}'");
            var result = await _storageService.DeleteIfExistsAsync(name);
            if (result == false) return NotFound($"Item '{name}' not found");
            return Ok(result);
        }
    }
}
