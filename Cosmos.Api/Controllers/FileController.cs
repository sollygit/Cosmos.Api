using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Download a file. This demo will generate a txt file.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Download a File by FileID")]
        public async Task<IActionResult> Download(int id)
        {
            var file = File(Encoding.ASCII.GetBytes("hello world"), "text/plain", $"file-{id}.txt");
            return await Task.FromResult(file);
        }

        /// <summary>
        /// Upload a file. This demo is dummy and only waits 2 seconds.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("single-file")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            _logger.LogInformation($"validating the file {file.FileName}");
            _logger.LogInformation("saving file");
            await Task.Delay(2000); // Validate file type/format/size, scan virus, save it to a storage
            _logger.LogInformation("file saved.");
            return await Task.FromResult(new OkObjectResult($"Filename '{file.FileName}' upload success"));
        }

        /// <summary>
        /// Upload two files. This demo is dummy and only waits 2 seconds.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Upload multiple files. This demo is dummy and only waits 2 seconds.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
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
