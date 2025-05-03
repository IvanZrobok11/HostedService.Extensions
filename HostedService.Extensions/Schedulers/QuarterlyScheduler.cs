using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation for quarterly execution
/// </summary>
public class QuarterlyScheduler : Scheduler
{
    private readonly int _month;

    /// <summary>
    /// Creates a quarterly scheduler for a specific month of each quarter
    /// </summary>
    /// <param name="month">Month of the quarter (1, 4, 7, or 10)</param>
    /// <param name="timeOfDay">Time of day for execution</param>
    /// <param name="timeZone">Time zone to use</param>
    public QuarterlyScheduler(int month, TimeOnly timeOfDay, TimeZoneInfo timeZone)
        : base(1, timeOfDay, timeZone)
    {
        if (month != 1 && month != 4 && month != 7 && month != 10)
        {
            throw new ArgumentException("Month must be 1, 4, 7, or 10 for quarterly schedule", nameof(month));
        }

        _month = month;
    }

    /// <summary>
    /// Calculates the next execution time
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    public override DateTime CalculateNextRunTime()
    {
        // Get current time in the specified time zone
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);

        // Create a scheduled time for the first day of the current month
        DateTime scheduledTime = new DateTime(
            now.Year,
            now.Month,
            1,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // Find the next quarter month
        int[] quarterMonths = { 1, 4, 7, 10 };
        int currentMonth = now.Month;

        // Find the nearest future quarter month
        int nextQuarterMonth = quarterMonths.FirstOrDefault(m => m >= currentMonth);
        if (nextQuarterMonth == 0) // No future month in this year
        {
            nextQuarterMonth = 1;  // January of next year
            scheduledTime = scheduledTime.AddYears(1);
        }

        // Set the month to the next quarter month
        scheduledTime = scheduledTime.AddMonths(nextQuarterMonth - scheduledTime.Month);

        // If this date has already passed, move to the next applicable quarter
        if (now > scheduledTime)
        {
            int nextIndex = Array.IndexOf(quarterMonths, nextQuarterMonth) + 1;
            if (nextIndex >= quarterMonths.Length)
            {
                // Move to the first quarter of next year
                scheduledTime = new DateTime(
                    scheduledTime.Year + 1,
                    1,
                    1,
                    TimeOfDay.Hour,
                    TimeOfDay.Minute,
                    TimeOfDay.Second,
                    now.Kind);
            }
            else
            {
                // Move to the next quarter in the same year
                scheduledTime = new DateTime(
                    scheduledTime.Year,
                    quarterMonths[nextIndex],
                    1,
                    TimeOfDay.Hour,
                    TimeOfDay.Minute,
                    TimeOfDay.Second,
                    now.Kind);
            }
        }

        return scheduledTime;
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the quarterly schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";
        string month = _month switch
        {
            1 => "January",
            4 => "April",
            7 => "July",
            10 => "October",
            _ => $"Month {_month}"
        };
        return $"Quarterly on the 1st day of {month} at {TimeOfDay}{timeZoneDisplay}";
    }
}
