using Cosmos.Api.HubConfig;
using Cosmos.Interface;
using Cosmos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.Api.Controllers
{
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
        public async Task<IActionResult> Get()
        {
            var chartData = await GetChartData();
            await _hub.Clients.All.SendAsync("transferChartData", chartData);
            return new OkObjectResult(chartData);
        }

        private async Task<Chart[]> GetChartData()
        {
            var lstChart = new List<Chart>();
            var candidates = await _candidateService.GetAll();
            var dictionary = GetTechnologyData(candidates);

            dictionary.Keys.ToList().ForEach(key => {
                lstChart.Add(new Chart { Label = key, Data = new int[] { dictionary[key] } });
            });
            
            return lstChart.ToArray();
        }

        private Dictionary<string, int> GetTechnologyData(IEnumerable<Candidate> candidates)
        {
            var dictionary = new Dictionary<string, int>();
            var technologies = candidates.Select(o => o.Technologies);

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

            return dictionary;
        }
    }
}
