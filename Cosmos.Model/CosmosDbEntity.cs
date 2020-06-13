using System;

namespace Cosmos.Model
{
    public abstract class CosmosDbEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
    }
}
