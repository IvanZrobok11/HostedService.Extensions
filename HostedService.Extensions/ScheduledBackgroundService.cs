using HostedService.Extensions.Schedulers;

namespace HostedService.Extensions;

public abstract class ScheduledBackgroundService(IServiceProvider services, ILogger logger) : TimePeriodicHostedService(services, logger)
{
    protected abstract Scheduler CallTrigger { get; }
    protected virtual bool ExecuteFirstRun { get; } = false;
    protected sealed override TimeSpan TimerPeriod => _timerPeriod;
    protected override TimeSpan? WaitFirsDelay => ExecuteFirstRun ? _timerPeriod : null;
    private TimeSpan _timerPeriod => CallTrigger.CalculateNextRunTime() - DateTime.UtcNow;

    protected sealed override async Task ExecuteAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduled Background Service is running. SchedulerType: {scheduleType}", CallTrigger.Type);

        await ExecuteAsync(cancellationToken, scope);
    }

    protected abstract Task ExecuteAsync(CancellationToken cancellationToken, IServiceScope scope);
}
