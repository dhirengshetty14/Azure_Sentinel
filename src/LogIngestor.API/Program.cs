// src/LogIngestor.API/Program.cs

using AzureSentinel.LogIngestor.API.Hubs;
using System.Threading.RateLimiting; // Required for Rate Limiting

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// This section adds a safety mechanism to our API.
builder.Services.AddRateLimiter(options =>
{
    // We're creating a "Fixed Window" limiter. This means we'll allow a certain number
    // of requests in a specific time window.
    options.AddFixedWindowLimiter(policyName: "fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100; // Allow 100 requests...
        limiterOptions.Window = TimeSpan.FromMinutes(1); // ...per minute.
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 5; // If we're busy, queue up to 5 requests before rejecting.
    });

    // This sets the default rejection response. Instead of a generic error, we're
    // telling the client exactly what happened (429 - Too Many Requests).
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// This tells our application to actually use the rate limiting rules we defined.
// It must be placed after UseRouting.
app.UseRateLimiter();
// ---

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowBlazorApp");
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.MapHub<LogHub>("/loghub");

app.Run();