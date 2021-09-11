using Cosmos.Api.HubConfig;
using Cosmos.Api.Models;
using Cosmos.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IHubContext<BoardHub> _hub;
        readonly ICloudTableService _cloudTableService;

        public BoardController(IHubContext<BoardHub> hub, ICloudTableService cloudTableService) =>
            (_hub, _cloudTableService) = (hub, cloudTableService);

        [HttpGet("[action]")]
        public async Task<IActionResult> Data()
        {
            var boardData = await _cloudTableService.GetBoardData();
            return new ObjectResult(boardData);
        }

        [HttpGet("GetNotice")]
        public async Task<IActionResult> GetNotice()
        {
            var entity = await _cloudTableService.GetNotice();
            return Ok(entity);
        }

        [HttpPost("CreateNotice")]
        public async Task<IActionResult> CreateNotice([FromBody] DataEntity dataEntity)
        {
            var entity = await _cloudTableService.CreateNotice(dataEntity);
            return Ok(entity);
        }

        [HttpGet]
        public async Task<IActionResult> TimerTriggerBoardData()
        {
            var boardData = await _cloudTableService.GetBoardData();
            await _hub.Clients.All.SendAsync("timerTriggerBoardData", boardData);
            return Ok(new { Message = "timerTriggerBoardData success" });
        }
    }
}
