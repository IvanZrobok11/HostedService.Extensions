using HostedService.Extensions;

namespace WebApp.Test;

public class BackupBackgroundService(IServiceProvider services, ILogger<TimePeriodicHostedService> logger)
    : TimePeriodicHostedService(services, logger)
{
    protected override TimeSpan TimerPeriod => TimeSpan.FromSeconds(1);
    protected override long? RunsLimit => 5;

    protected override async Task ExecuteAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync("Backuping...");
    }
}
