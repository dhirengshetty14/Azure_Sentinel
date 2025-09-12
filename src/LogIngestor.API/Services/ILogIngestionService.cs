// src/LogIngestionService/Services/ILogIngestionService.cs

using AzureSentinel.Core.Models;

namespace AzureSentinel.LogIngestor.API.Services;

public interface ILogIngestionService
{
    Task ProcessAndDistributeLogAsync(LogEntry logEntry);
}