using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation for work hours (9 AM to 5 PM, Monday to Friday)
/// </summary>
public class WorkHoursScheduler : Scheduler
{
    private readonly int _intervalMinutes;

    /// <summary>
    /// Creates a scheduler for work hours
    /// </summary>
    /// <param name="intervalMinutes">Interval in minutes</param>
    /// <param name="timeZone">Time zone to use</param>
    public WorkHoursScheduler(int intervalMinutes, TimeZoneInfo timeZone)
        : base(intervalMinutes, timeZone)
    {
        _intervalMinutes = intervalMinutes;
    }

    /// <summary>
    /// Calculates the next execution time during work hours
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    public override DateTime CalculateNextRunTime()
    {
        // Get current time in the specified time zone
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);

        // Check if current day is a weekend
        bool isWeekend = now.DayOfWeek == System.DayOfWeek.Saturday || now.DayOfWeek == System.DayOfWeek.Sunday;

        // Check if current time is outside of work hours (9 AM to 5 PM)
        bool isOutsideWorkHours = now.Hour < 9 || now.Hour >= 17;

        if (isWeekend || isOutsideWorkHours)
        {
            // If it's a weekend or outside of work hours, move to 9 AM on the next business day
            DateTime nextBusinessDay = now;

            // If it's a weekend or after hours, move to the next business day
            if (isWeekend || now.Hour >= 17)
            {
                // Add days until we reach a business day
                do
                {
                    nextBusinessDay = nextBusinessDay.AddDays(1);
                }
                while (nextBusinessDay.DayOfWeek == System.DayOfWeek.Saturday || nextBusinessDay.DayOfWeek == System.DayOfWeek.Sunday);

                // Set time to 9 AM
                nextBusinessDay = new DateTime(
                    nextBusinessDay.Year,
                    nextBusinessDay.Month,
                    nextBusinessDay.Day,
                    9, 0, 0,
                    nextBusinessDay.Kind);
            }
            else // Before work hours on a business day
            {
                // Set time to 9 AM on the current day
                nextBusinessDay = new DateTime(
                    nextBusinessDay.Year,
                    nextBusinessDay.Month,
                    nextBusinessDay.Day,
                    9, 0, 0,
                    nextBusinessDay.Kind);
            }

            return nextBusinessDay;
        }

        // We're during work hours on a business day

        // Round current minute down to nearest multiple of interval
        int currentMinute = now.Minute;
        int currentSecond = now.Second;
        int currentMillisecond = now.Millisecond;

        int minuteInterval = _intervalMinutes;
        int nearestMultiple = (currentMinute / minuteInterval) * minuteInterval;

        // Calculate the next execution time
        DateTime nextRun = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            nearestMultiple,
            0,
            0,
            now.Kind);

        // If we're already past the nearest multiple, add the interval
        if (nearestMultiple < currentMinute ||
            (nearestMultiple == currentMinute && (currentSecond > 0 || currentMillisecond > 0)))
        {
            nextRun = nextRun.AddMinutes(minuteInterval);
        }

        // Check if the next run would be after work hours
        if (nextRun.Hour >= 17)
        {
            // Move to 9 AM on the next business day
            do
            {
                nextRun = nextRun.AddDays(1);
            }
            while (nextRun.DayOfWeek == System.DayOfWeek.Saturday || nextRun.DayOfWeek == System.DayOfWeek.Sunday);

            nextRun = new DateTime(
                nextRun.Year,
                nextRun.Month,
                nextRun.Day,
                9, 0, 0,
                nextRun.Kind);
        }

        return nextRun;
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the work hours schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";
        return $"Every {_intervalMinutes} minute(s) during work hours (9 AM - 5 PM, Monday - Friday){timeZoneDisplay}";
    }
}
