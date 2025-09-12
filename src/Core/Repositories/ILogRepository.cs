using AzureSentinel.Core.Models;

namespace AzureSentinel.Core.Repositories;

// This interface defines the contract for data operations on our logs.
// This allows us to swap out the database implementation (e.g., from Cosmos DB to something else)
// without changing our business logic.
public interface ILogRepository
{
    Task AddLogAsync(LogEntry logEntry);
    Task<IEnumerable<LogEntry>> GetRecentLogsAsync(int count = 100);
    Task<LogAnalyticsSummary> GetLogSummaryAsync(DateTime since);
}

// A simple model to hold our summary data
public record LogAnalyticsSummary(int ErrorCount, int WarningCount, int InfoCount, int TotalCount);