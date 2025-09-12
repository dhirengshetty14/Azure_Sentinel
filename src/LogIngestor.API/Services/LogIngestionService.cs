// src/LogIngestionService/Services/LogIngestionService.cs

using AzureSentinel.Core.Models;
using AzureSentinel.LogIngestor.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AzureSentinel.LogIngestor.API.Services;

public class LogIngestionService : ILogIngestionService
{
    private readonly IHubContext<LogHub> _logHubContext;
    private readonly ILogger<LogIngestionService> _logger;

    public LogIngestionService(IHubContext<LogHub> logHubContext, ILogger<LogIngestionService> logger)
    {
        _logHubContext = logHubContext;
        _logger = logger;
    }

    public async Task ProcessAndDistributeLogAsync(LogEntry logEntry)
    {
        // In the future, more complex logic could go here, like:
        // - Saving the log to a database (Cosmos DB)
        // - Sending the log to Azure Event Hubs
        // - Checking for specific keywords to trigger alerts

        _logger.LogInformation("Processing and distributing log from {SourceApplication}", logEntry.SourceApplication);

        // For now, it just broadcasts the log via SignalR
        await _logHubContext.Clients.All.SendAsync("ReceiveLog", logEntry);
    }
}