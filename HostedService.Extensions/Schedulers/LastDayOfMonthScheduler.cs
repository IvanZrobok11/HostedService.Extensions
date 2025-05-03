using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation for the last day of each month
/// </summary>
public class LastDayOfMonthScheduler : Scheduler
{
    /// <summary>
    /// Creates a scheduler for the last day of each month
    /// </summary>
    /// <param name="timeOfDay">Time of day for execution</param>
    /// <param name="timeZone">Time zone to use</param>
    public LastDayOfMonthScheduler(TimeOnly timeOfDay, TimeZoneInfo timeZone)
        : base(timeOfDay, timeZone)
    {
    }

    /// <summary>
    /// Calculates the next execution time on the last day of a month
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    public override DateTime CalculateNextRunTime()
    {
        // Get current time in the specified time zone
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);

        // Get the last day of the current month
        int lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);

        // Create a scheduled time for the last day of the current month
        DateTime scheduledTime = new DateTime(
            now.Year,
            now.Month,
            lastDayOfMonth,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // If this date has already passed, move to the last day of the next month
        if (now > scheduledTime)
        {
            DateTime nextMonth = now.AddMonths(1);
            int lastDayOfNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);

            scheduledTime = new DateTime(
                nextMonth.Year,
                nextMonth.Month,
                lastDayOfNextMonth,
                TimeOfDay.Hour,
                TimeOfDay.Minute,
                TimeOfDay.Second,
                now.Kind);
        }

        return scheduledTime;
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the last day of month schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";
        return $"On the last day of every month at {TimeOfDay}{timeZoneDisplay}";
    }
}
