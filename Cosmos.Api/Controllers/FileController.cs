using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cosmos.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        readonly ILogger<FileController> _logger;
        readonly IStorageService _storageService;

        public FileController(ILogger<FileController> logger, IStorageService storageService) =>
            (_logger, _storageService) = (logger, storageService);

        [HttpGet("{id}", Name = "Download a File by FileID")]
        public async Task<IActionResult> Download(int id)
        {
            var file = File(Encoding.ASCII.GetBytes("hello world"), "text/plain", $"file-{id}.txt");
            return await Task.FromResult(file);
        }

        [HttpGet("Blob/items")]
        public async Task<IActionResult> BlobItems()
        {
            var items = await _storageService.GetBlobItemsAsync();
            return Ok(items);
        }

        [HttpPost("single-file")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null) return BadRequest("A file is required!");
            _logger.LogInformation($"Upload file: '{file.FileName}'");
            var filename = await _storageService.UploadAsync(file);
            return Ok(filename);
        }

        [HttpPost("two-files")]
        public async Task<IActionResult> Upload(IFormFile file1, IFormFile file2)
        {
            _logger.LogInformation($"validating the file {file1.FileName}");
            _logger.LogInformation($"validating the file {file2.FileName}");
            _logger.LogInformation("saving files");
            await Task.Delay(2000);
            _logger.LogInformation("files saved.");
            return await Task.FromResult(new OkObjectResult("two-files upload success"));
        }

        [HttpPost("multiple-files")]
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

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"deleting file ID=[{id}]");
            await Task.Delay(1500);
            return await Task.FromResult(new OkObjectResult(true));
        }
    }
}
