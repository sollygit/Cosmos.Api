# CosmosDB API in .NET

[RESTful API demo with Swagger](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-concept-azure-functions)

| Real-time processing architecture |
| --------------------------------- |
| ![GraphiQL](https://user-images.githubusercontent.com/30820950/73920150-91a47500-4910-11ea-9a82-b82cc39e1b97.png) |

- Build real-time Apps with Azure Functions and Azure SignalR Service

- A change is made in a Cosmos DB collection
  - New document added to collection.
  - Existing document updated.
- The change event is propagated to the Cosmos DB change feed
- An Azure Function is triggered by the change event using the Cosmos DB trigger
  - Functions are running locally `http://localhost:7070` or on the cloud
- The SignalR Service output binding publishes a message to SignalR Service
- SignalR Service publishes the message to all connected clients

```bash
dotnet run
```
