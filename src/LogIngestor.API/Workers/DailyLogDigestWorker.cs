// src/LogIngestor.API/Workers/DailyLogDigestWorker.cs

using Azure;
using Azure.Communication.Email;
using AzureSentinel.Core.Models;
using AzureSentinel.LogIngestor.API.Services; // We'll need a way to access logs

namespace AzureSentinel.LogIngestor.API.Workers;

public class DailyLogDigestWorker : BackgroundService
{
    private readonly ILogger<DailyLogDigestWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public DailyLogDigestWorker(ILogger<DailyLogDigestWorker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Log Digest Worker starting up.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                _logger.LogInformation("Generating daily log digest...");

                //simulate getting logs from our service.
                var logSummary = GenerateSummary();

                // Send the summary via email using Azure Communication Services
                await SendDigestEmailAsync(logSummary);

            }
            catch (TaskCanceledException)
            {
                // This is expected when the application is shutting down.
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Daily Log Digest Worker.");
            }
        }

        _logger.LogInformation("Daily Log Digest Worker shutting down.");
    }

    private string GenerateSummary()
    {
        
        var errorCount = new Random().Next(0, 15);
        var warningCount = new Random().Next(20, 100);
        var infoCount = new Random().Next(500, 2000);

        return $"Hello! Here is your daily log summary:\n- Errors: {errorCount}\n- Warnings: {warningCount}\n- Info: {infoCount}\n\nHave a great day!";
    }

    private async Task SendDigestEmailAsync(string summaryContent)
    {
        var connectionString = _configuration["AzureCommunicationServices:ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Azure Communication Services connection string is not configured. Skipping email.");
            return;
        }

        var emailClient = new EmailClient(connectionString);
        var senderAddress = _configuration["AzureCommunicationServices:SenderAddress"];
        var recipientAddress = _configuration["AzureCommunicationServices:RecipientAddress"];


        EmailSendOperation emailSendOperation = await emailClient.SendAsync(
            WaitUntil.Completed,
            sender: senderAddress,
            recipient: recipientAddress,
            subject: "Your Daily Log Digest - Azure Sentinel",
            htmlContent: $"<html><body><h1>Log Digest</h1><p>{summaryContent.Replace("\n", "<br>")}</p></body></html>",
            plainTextContent: summaryContent);

        _logger.LogInformation("Digest email sent with status: {Status}", emailSendOperation.Value.Status);
    }
}