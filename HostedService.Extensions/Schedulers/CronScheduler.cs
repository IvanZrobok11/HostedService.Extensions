using HostedService.Extensions.Schedulers;
/// <summary>
/// Schedule implementation based on cron expressions
/// </summary>
public class CronScheduler : Scheduler
{
    private readonly string _cronExpression;
    private readonly int[] _minutes;
    private readonly int[] _hours;
    private readonly int[] _daysOfMonth;
    private readonly int[] _months;
    private readonly int[] _daysOfWeek;

    /// <summary>
    /// Creates a scheduler based on a cron expression
    /// </summary>
    /// <param name="cronExpression">Cron expression (minute, hour, day of month, month, day of week)</param>
    /// <param name="timeZone">Time zone to use</param>
    public CronScheduler(string cronExpression, TimeZoneInfo timeZone)
        : base(new TimeOnly(0, 0, 0), timeZone)
    {
        _cronExpression = cronExpression;

        var parts = cronExpression.Split(' ');
        if (parts.Length != 5)
        {
            throw new ArgumentException("Cron expression must have 5 parts: minute, hour, day of month, month, day of week", nameof(cronExpression));
        }

        _minutes = ParseCronPart(parts[0], 0, 59);
        _hours = ParseCronPart(parts[1], 0, 23);
        _daysOfMonth = ParseCronPart(parts[2], 1, 31);
        _months = ParseCronPart(parts[3], 1, 12);
        _daysOfWeek = ParseCronPart(parts[4], 0, 6); // 0-6 for Sunday-Saturday
    }

    /// <summary>
    /// Parses a part of the cron expression into an array of valid values
    /// </summary>
    /// <param name="part">Cron expression part</param>
    /// <param name="min">Minimum valid value</param>
    /// <param name="max">Maximum valid value</param>
    /// <returns>Array of valid values</returns>
    private int[] ParseCronPart(string part, int min, int max)
    {
        if (part == "*")
        {
            // All values in range
            return Enumerable.Range(min, max - min + 1).ToArray();
        }

        List<int> values = new List<int>();

        foreach (var segment in part.Split(','))
        {
            if (segment.Contains('/')) // Step values
            {
                var rangeParts = segment.Split('/');
                int step = int.Parse(rangeParts[1]);

                int rangeMin = min;
                int rangeMax = max;

                if (rangeParts[0] != "*")
                {
                    if (rangeParts[0].Contains('-'))
                    {
                        var rangeBounds = rangeParts[0].Split('-');
                        rangeMin = int.Parse(rangeBounds[0]);
                        rangeMax = int.Parse(rangeBounds[1]);
                    }
                    else
                    {
                        rangeMin = int.Parse(rangeParts[0]);
                    }
                }

                for (int i = rangeMin; i <= rangeMax; i += step)
                {
                    values.Add(i);
                }
            }
            else if (segment.Contains('-')) // Range
            {
                var rangeParts = segment.Split('-');
                int rangeMin = int.Parse(rangeParts[0]);
                int rangeMax = int.Parse(rangeParts[1]);

                for (int i = rangeMin; i <= rangeMax; i++)
                {
                    values.Add(i);
                }
            }
            else // Single value
            {
                values.Add(int.Parse(segment));
            }
        }

        return values.Distinct().OrderBy(v => v).ToArray();
    }

    /// <summary>
    /// Calculates the next execution time according to the cron expression
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    public override DateTime CalculateNextRunTime()
    {
        // Get current time in the specified time zone
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);
        DateTime candidate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind).AddMinutes(1);

        // Try to find a valid time within the next year (to avoid infinite loops)
        DateTime maxDate = now.AddYears(1);

        while (candidate < maxDate)
        {
            // Check if all parts of the candidate time match the cron expression
            if (!_months.Contains(candidate.Month))
            {
                candidate = new DateTime(candidate.Year, candidate.Month, 1, 0, 0, 0, candidate.Kind).AddMonths(1);
                continue;
            }

            if (!_daysOfMonth.Contains(candidate.Day))
            {
                candidate = candidate.AddDays(1).Date;
                continue;
            }

            int dayOfWeek = (int)candidate.DayOfWeek;
            if (!_daysOfWeek.Contains(dayOfWeek))
            {
                candidate = candidate.AddDays(1).Date;
                continue;
            }

            if (!_hours.Contains(candidate.Hour))
            {
                candidate = candidate.AddHours(1 - candidate.Hour % 1);
                continue;
            }

            if (!_minutes.Contains(candidate.Minute))
            {
                candidate = candidate.AddMinutes(1);
                continue;
            }

            // If we get here, the candidate time matches all parts of the cron expression
            return candidate;
        }

        // If no valid time found within a year, return maxDate
        return maxDate;
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the cron schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";
        return $"Cron schedule: {_cronExpression}{timeZoneDisplay}";
    }
}
