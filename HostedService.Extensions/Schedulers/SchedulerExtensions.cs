using HostedService.Extensions.Schedulers;

/// <summary>
/// Extension methods for the Scheduler class
/// </summary>
public static class SchedulerExtensions
{
    /// <summary>
    /// Creates a schedule for business days (Monday to Friday) at a specific time
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object for business days</returns>
    public static Scheduler BusinessDays(this Scheduler scheduler, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        Scheduler.ValidateTime(hour, minute, second);
        var businessDays = new[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        return Scheduler.Weekly(businessDays, new TimeOnly(hour, minute, second), timeZone);
    }

    /// <summary>
    /// Creates a schedule for weekends (Saturday and Sunday) at a specific time
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object for weekends</returns>
    public static Scheduler Weekends(this Scheduler scheduler, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        Scheduler.ValidateTime(hour, minute, second);
        var weekendDays = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

        return Scheduler.Weekly(weekendDays, new TimeOnly(hour, minute, second), timeZone);
    }

    /// <summary>
    /// Creates a schedule for quarterly execution (first day of each quarter) at a specific time
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>List of four monthly schedules for the first day of each quarter</returns>
    public static IEnumerable<Scheduler> Quarterly(this Scheduler scheduler, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        Scheduler.ValidateTime(hour, minute, second);
        var quarterMonths = new[] { 1, 4, 7, 10 }; // January, April, July, October

        return quarterMonths.Select(month =>
            new QuarterlyScheduler(month, new TimeOnly(hour, minute, second), timeZone ?? TimeZoneInfo.Utc));
    }

    /// <summary>
    /// Creates a schedule that runs on the first day of each month at a specific time
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule for the first day of each month</returns>
    public static Scheduler FirstDayOfMonth(this Scheduler scheduler, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        return Scheduler.MonthAt(1, hour, minute, second, timeZone);
    }

    /// <summary>
    /// Creates a schedule that runs on the last day of each month at a specific time.
    /// Correctly handles varying month lengths (28/29/30/31 days).
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule for the last day of each month</returns>
    public static Scheduler LastDayOfMonth(this Scheduler scheduler, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        return new LastDayOfMonthScheduler(new TimeOnly(hour, minute, second), timeZone ?? TimeZoneInfo.Utc);
    }

    /// <summary>
    /// Creates a schedule that runs once at a specific date and time
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="dateTime">Date and time for one-time execution</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule for one-time execution</returns>
    public static Scheduler Once(this Scheduler scheduler, DateTime dateTime, TimeZoneInfo? timeZone = null)
    {
        return new OneTimeScheduler(dateTime, timeZone ?? TimeZoneInfo.Utc);
    }

    /// <summary>
    /// Creates a cron-like schedule based on a cron expression using a popular cron parsing library
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="cronExpression">Cron expression (e.g. "0 0 * * *" for daily at midnight)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule based on cron expression</returns>
    /// <remarks>
    /// This method relies on the Cronos library. Make sure to install it via NuGet:
    /// PM> Install-Package Cronos
    /// </remarks>
    public static Scheduler Cron(this Scheduler scheduler, string cronExpression, TimeZoneInfo? timeZone = null)
    {
        return new CronScheduler(cronExpression, timeZone ?? TimeZoneInfo.Utc);
    }

    /// <summary>
    /// Creates a workday schedule (Monday to Friday, 9 AM to 5 PM) with specified interval
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="intervalMinutes">Interval in minutes</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule for workday hours</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is out of valid range</exception>
    public static Scheduler WorkHours(this Scheduler scheduler, int intervalMinutes, TimeZoneInfo? timeZone = null)
    {
        if (intervalMinutes < 1 || intervalMinutes > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalMinutes), "Interval must be between 1 and 60 minutes");
        }

        return new WorkHoursScheduler(intervalMinutes, timeZone ?? TimeZoneInfo.Utc);
    }

    /// <summary>
    /// Combines multiple schedules into a single schedule that triggers when any of the individual schedules would trigger
    /// </summary>
    /// <param name="scheduler">The scheduler instance</param>
    /// <param name="schedules">Collection of schedules to combine</param>
    /// <returns>Combined schedule</returns>
    public static Scheduler Combine(this Scheduler scheduler, params Scheduler[] schedules)
    {
        return new CombinedScheduler(schedules.Append(scheduler));
    }
}
