using System.Text.Json.Serialization;

namespace Cosmos.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResidentialType
    {
        InState,
        OutOfState,
        International
    }
}
