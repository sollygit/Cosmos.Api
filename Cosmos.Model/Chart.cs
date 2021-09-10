using Newtonsoft.Json;

namespace Cosmos.Model
{
    public class Chart
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("data")]
        public int[] Data { get; set; }
        
        public Chart()
        {
            Data = new int[] { };
        }
    }
}
