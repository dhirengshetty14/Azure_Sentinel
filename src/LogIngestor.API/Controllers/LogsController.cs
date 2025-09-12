using AzureSentinel.Core.Models;
using AzureSentinel.LogIngestor.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AzureSentinel.LogIngestor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly IHubContext<LogHub> _logHubContext;
    private readonly ILogger<LogsController> _logger;

    public LogsController(IHubContext<LogHub> logHubContext, ILogger<LogsController> logger)
    {
        _logHubContext = logHubContext;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PostLog([FromBody] LogEntry logEntry)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        _logger.LogInformation("Received log from {SourceApplication}", logEntry.SourceApplication);
        await _logHubContext.Clients.All.SendAsync("ReceiveLog", logEntry);
        return Accepted();
    }
}