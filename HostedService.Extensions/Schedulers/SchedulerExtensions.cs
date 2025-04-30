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
    public static Scheduler Combine(this Scheduler scheduler, IEnumerable<Scheduler> schedules)
    {
        return new CombinedScheduler(schedules.Append(scheduler));
    }
}

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