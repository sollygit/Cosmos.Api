using Cosmos.Api.HubConfig;
using Cosmos.Api.Services;
using Cosmos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly IHubContext<ChartHub> _hub;
        readonly ICandidateService _candidateService;

        public ChartController(IHubContext<ChartHub> hub, ICandidateService candidateService)
        {
            _hub = hub;
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<IActionResult> TransferChartData()
        {
            var chartData = await GetChartData();
            await _hub.Clients.All.SendAsync("transferChartData", chartData);
            return Ok(new { Message = "transferChartData success" });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Data()
        {
            var chartData = await GetChartData();
            return new ObjectResult(chartData);
        }

        async Task<IEnumerable<Chart>> GetChartData()
        {
            var candidates = await _candidateService.GetAsync();
            var technologies = candidates.Select(o => o.Technologies);
            var lstChart = new List<Chart>();
            var dictionary = new Dictionary<string, int>();

            foreach (var arr in technologies)
            {
                arr.ToList().ForEach(key => {
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, 1);
                    }
                    else
                    {
                        dictionary[key]++;
                    }
                });
            }

            dictionary.Keys.ToList().ForEach(key => {
                lstChart.Add(new Chart { Label = key, Data = new int[] { dictionary[key] } });
            });

            return lstChart.OrderBy(o => o.Label);
        }
    }
}