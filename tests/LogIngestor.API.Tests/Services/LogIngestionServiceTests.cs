// tests/LogIngestor.API.Tests/Services/LogIngestionServiceTests.cs

using AzureSentinel.Core.Models;
using AzureSentinel.LogIngestor.API.Hubs;
using AzureSentinel.LogIngestor.API.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LogIngestor.API.Tests.Services;

public class LogIngestionServiceTests
{
    [Fact]
    public async Task ProcessAndDistributeLogAsync_Always_SendsToSignalR()
    {
        // Arrange: Set up the test conditions
        var mockHubContext = new Mock<IHubContext<LogHub>>();
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        var mockLogger = new Mock<ILogger<LogIngestionService>>();

        // This is how we mock the SignalR chain: HubContext -> Clients -> All -> SendAsync
        mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);

        var service = new LogIngestionService(mockHubContext.Object, mockLogger.Object);
        var testLogEntry = new LogEntry { Message = "Test log" };

        // Act: Call the method we are testing
        await service.ProcessAndDistributeLogAsync(testLogEntry);

        // We are checking if the SendAsync method was called exactly once with the correct arguments.
        mockClientProxy.Verify(
            x => x.SendCoreAsync("ReceiveLog", It.Is<object[]>(o => ((LogEntry)o[0]).Message == "Test log"), default),
            Times.Once
        );
    }
}