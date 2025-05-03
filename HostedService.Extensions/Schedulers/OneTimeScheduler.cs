using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation for one-time execution
/// </summary>
public class OneTimeScheduler : Scheduler
{
    private readonly DateTime _executionTime;

    /// <summary>
    /// Creates a one-time scheduler for a specific date and time
    /// </summary>
    /// <param name="executionTime">Date and time for execution</param>
    /// <param name="timeZone">Time zone to use</param>
    public OneTimeScheduler(DateTime executionTime, TimeZoneInfo timeZone)
        : base(new TimeOnly(executionTime.Hour, executionTime.Minute, executionTime.Second), timeZone)
    {
        _executionTime = TimeZoneInfo.ConvertTime(executionTime, timeZone);
    }

    /// <summary>
    /// Calculates the next execution time
    /// </summary>
    /// <returns>The scheduled execution time if it's in the future, otherwise DateTime.MaxValue</returns>
    public override DateTime CalculateNextRunTime()
    {
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);
        return now > _executionTime ? DateTime.MaxValue : _executionTime;
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the one-time schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";
        return $"Once at {_executionTime:yyyy-MM-dd HH:mm:ss}{timeZoneDisplay}";
    }
}
