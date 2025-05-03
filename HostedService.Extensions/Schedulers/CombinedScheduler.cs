using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation that combines multiple schedules
/// </summary>
public class CombinedScheduler : Scheduler
{
    private readonly IEnumerable<Scheduler> _schedules;

    /// <summary>
    /// Creates a scheduler that combines multiple schedules
    /// </summary>
    /// <param name="schedules">Collection of schedules to combine</param>
    public CombinedScheduler(IEnumerable<Scheduler> schedules)
        : base(new TimeOnly(0, 0, 0), TimeZoneInfo.Utc)
    {
        _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));

        if (!_schedules.Any())
        {
            throw new ArgumentException("At least one schedule must be provided", nameof(schedules));
        }
    }

    /// <summary>
    /// Calculates the next execution time as the earliest time from all combined schedules
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    public override DateTime CalculateNextRunTime()
    {
        return _schedules.Min(s => s.CalculateNextRunTime());
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the combined schedule</returns>
    public override string ToString()
    {
        return $"Combined schedule with {_schedules.Count()} schedule(s)";
    }
}