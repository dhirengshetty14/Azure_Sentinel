using Azure.Cosmos;
using AzureSentinel.Core.Models;
using AzureSentinel.Core.Repositories;

namespace AzureSentinel.LogIngestor.API.Repositories;

public class CosmosDbLogRepository : ILogRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private readonly ILogger<CosmosDbLogRepository> _logger;

    public CosmosDbLogRepository(CosmosClient cosmosClient, IConfiguration configuration, ILogger<CosmosDbLogRepository> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
        var databaseName = configuration["CosmosDb:DatabaseName"];
        var containerName = configuration["CosmosDb:ContainerName"];
        _container = _cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task AddLogAsync(LogEntry logEntry)
    {
        try
        {
            // Create the item in our container, using its SourceApplication as the partition key.
            await _container.CreateItemAsync(logEntry, new PartitionKey(logEntry.SourceApplication));
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "An error occurred while adding a log to Cosmos DB.");
            // In a real app, you might add retry logic here.
            throw;
        }
    }
    
    // We will implement these methods in the next step.
    public Task<IEnumerable<LogEntry>> GetRecentLogsAsync(int count = 100)
    {
        throw new NotImplementedException();
    }

    public Task<LogAnalyticsSummary> GetLogSummaryAsync(DateTime since)
    {
        throw new NotImplementedException();
    }
}