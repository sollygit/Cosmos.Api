using Cosmos.Model;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Cosmos.Api.HubConfig
{
    public class ChartHub : Hub
    {
        public async Task BroadcastChartData(Chart[] data) => await Clients.All.SendAsync("broadcastChartData", data);
    }
}
