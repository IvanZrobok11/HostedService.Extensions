using HostedService.Extensions;
using HostedService.Extensions.Schedulers;

namespace WebApp.Test;

public class EmailNotificationSchedulerService(IServiceProvider services, ILogger<EmailNotificationSchedulerService> logger)
    : ScheduledBackgroundService(services, logger)
{
    protected override Scheduler Scheduler => Scheduler.Minutely(5);
    protected override bool ExecuteFirstRun => true;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken, IServiceScope scope)
    {
        await Console.Out.WriteLineAsync("EmailNotificationSchedulerService");
    }
}