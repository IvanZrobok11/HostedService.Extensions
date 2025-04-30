using HealthChecks.UI.Client;
using HostedService.Extensions.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebApp.Test;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registration of various background services
builder.Services.AddHostedService<BackupBackgroundService>();
builder.Services.AddHostedService<EmailNotificationSchedulerService>();

// Adding Health Checks
builder.Services.AddHealthChecks().AddBackgroundServicesCheck();

// Adding UI for Health Checks
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(10);
    options.MaximumHistoryEntriesPerEndpoint(60);
    options.AddHealthCheckEndpoint("My system", "/health");
})
.AddInMemoryStorage();

var app = builder.Build();
app.UseHttpsRedirection();

// Setting up Health Checks endpoints
app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthui";
    options.ApiPath = "/health-ui-api";
});

app.Run();