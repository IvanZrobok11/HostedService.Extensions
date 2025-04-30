namespace HostedService.Extensions.Schedulers;

/// <summary>
/// Type of schedule for executing background tasks
/// </summary>
public enum ScheduleType
{
    Secondly,    // Added Secondly schedule type
    Minutely,
    Hourly,
    Daily,
    Weekly,
    Monthly
}

/// <summary>
/// Class for easy creation and configuration of task execution schedules.
/// Provides a fluent interface to create various schedule types.
/// </summary>
public class Scheduler : IEquatable<Scheduler>
{
    /// <summary>
    /// Type of schedule
    /// </summary>
    public ScheduleType Type { get; private set; }

    /// <summary>
    /// Time of day for execution
    /// </summary>
    public TimeOnly TimeOfDay { get; private set; }

    /// <summary>
    /// Day of the week for weekly schedule
    /// </summary>
    public DayOfWeek? DayOfWeek { get; private set; }

    /// <summary>
    /// List of days of the week for multiple day weekly schedules
    /// </summary>
    public HashSet<DayOfWeek>? DaysOfWeek { get; private set; }

    /// <summary>
    /// Day of the month for monthly schedule
    /// </summary>
    public int? DayOfMonth { get; private set; }

    /// <summary>
    /// Time zone used for scheduling
    /// </summary>
    public TimeZoneInfo TimeZone { get; private set; }

    /// <summary>
    /// Second interval for secondly schedule
    /// </summary>
    public int? SecondInterval { get; private set; }

    /// <summary>
    /// Minute interval for minutely schedule
    /// </summary>
    public int? MinuteInterval { get; private set; }

    /// <summary>
    /// Hour interval for hourly schedule
    /// </summary>
    public int? HourInterval { get; private set; }

    /// <summary>
    /// Minute of the hour for hourly schedule
    /// </summary>
    public int? MinuteOfHour { get; private set; }

    /// <summary>
    /// Constructor for daily schedule
    /// </summary>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Daily;
        TimeOfDay = timeOfDay;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for weekly schedule with a single day
    /// </summary>
    /// <param name="dayOfWeek">Day of week</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(DayOfWeek dayOfWeek, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Weekly;
        DayOfWeek = dayOfWeek;
        TimeOfDay = timeOfDay;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for weekly schedule with multiple days
    /// </summary>
    /// <param name="daysOfWeek">Days of week</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <exception cref="ArgumentException">Thrown when days of week are empty</exception>
    protected Scheduler(IEnumerable<DayOfWeek> daysOfWeek, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        if (!daysOfWeek.Any())
        {
            throw new ArgumentException("At least one day of the week must be specified", nameof(daysOfWeek));
        }

        Type = ScheduleType.Weekly;
        DaysOfWeek = new HashSet<DayOfWeek>(daysOfWeek);
        TimeOfDay = timeOfDay;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for monthly schedule
    /// </summary>
    /// <param name="dayOfMonth">Day of month</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(int dayOfMonth, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Monthly;
        DayOfMonth = dayOfMonth;
        TimeOfDay = timeOfDay;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for hourly schedule
    /// </summary>
    /// <param name="hourInterval">Hour interval</param>
    /// <param name="minuteOfHour">Minute of the hour</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(int hourInterval, int minuteOfHour, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Hourly;
        HourInterval = hourInterval;
        MinuteOfHour = minuteOfHour;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for minutely schedule
    /// </summary>
    /// <param name="minuteInterval">Minute interval</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(int minuteInterval, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Minutely;
        MinuteInterval = minuteInterval;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Constructor for secondly schedule
    /// </summary>
    /// <param name="secondInterval">Second interval</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    protected Scheduler(int secondInterval, bool isSecondly, TimeZoneInfo? timeZone = null)
    {
        Type = ScheduleType.Secondly;
        SecondInterval = secondInterval;
        TimeZone = timeZone ?? TimeZoneInfo.Utc;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a schedule for monthly execution
    /// </summary>
    /// <param name="dayOfMonth">Day of month (1-31)</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when day of month is not between 1 and 31</exception>
    public static Scheduler Monthly(int dayOfMonth, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        if (dayOfMonth < 1 || dayOfMonth > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Day of month must be between 1 and 31");
        }

        return new Scheduler(dayOfMonth, timeOfDay, timeZone);
    }

    /// <summary>
    /// Creates a schedule for weekly execution
    /// </summary>
    /// <param name="dayOfWeek">Day of week</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    public static Scheduler Weekly(DayOfWeek dayOfWeek, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        return new Scheduler(dayOfWeek, timeOfDay, timeZone);
    }

    /// <summary>
    /// Creates a schedule for weekly execution on multiple days
    /// </summary>
    /// <param name="daysOfWeek">Days of week</param>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentException">Thrown when days of week are empty</exception>
    public static Scheduler Weekly(IEnumerable<DayOfWeek> daysOfWeek, TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        return new Scheduler(daysOfWeek, timeOfDay, timeZone);
    }

    /// <summary>
    /// Creates a schedule for daily execution
    /// </summary>
    /// <param name="timeOfDay">Time of day</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    public static Scheduler Daily(TimeOnly timeOfDay, TimeZoneInfo? timeZone = null)
    {
        return new Scheduler(timeOfDay, timeZone);
    }

    /// <summary>
    /// Creates a schedule for hourly execution
    /// </summary>
    /// <param name="hourInterval">Hour interval (1-23)</param>
    /// <param name="minuteOfHour">Minute of the hour (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of valid ranges</exception>
    public static Scheduler Hourly(int hourInterval, int minuteOfHour, TimeZoneInfo? timeZone = null)
    {
        if (hourInterval < 1 || hourInterval > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hourInterval), "Hour interval must be between 1 and 23");
        }

        if (minuteOfHour < 0 || minuteOfHour > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minuteOfHour), "Minute of hour must be between 0 and 59");
        }

        return new Scheduler(hourInterval, minuteOfHour, timeZone);
    }

    /// <summary>
    /// Creates a schedule for every hour execution
    /// </summary>
    /// <param name="minuteOfHour">Minute of the hour (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when minute is out of valid range</exception>
    public static Scheduler EveryHour(int minuteOfHour, TimeZoneInfo? timeZone = null)
    {
        return Hourly(1, minuteOfHour, timeZone);
    }

    /// <summary>
    /// Creates a schedule for minutely execution
    /// </summary>
    /// <param name="minuteInterval">Minute interval (1-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when minute interval is out of valid range</exception>
    public static Scheduler Minutely(int minuteInterval, TimeZoneInfo? timeZone = null)
    {
        if (minuteInterval < 1 || minuteInterval > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(minuteInterval), "Minute interval must be between 1 and 59");
        }

        return new Scheduler(minuteInterval, timeZone);
    }

    /// <summary>
    /// Creates a schedule for every minute execution
    /// </summary>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    public static Scheduler EveryMinute(TimeZoneInfo? timeZone = null)
    {
        return Minutely(1, timeZone);
    }

    /// <summary>
    /// Creates a schedule for secondly execution
    /// </summary>
    /// <param name="secondInterval">Second interval (1-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when second interval is out of valid range</exception>
    public static Scheduler Secondly(int secondInterval, TimeZoneInfo? timeZone = null)
    {
        if (secondInterval < 1 || secondInterval > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(secondInterval), "Second interval must be between 1 and 59");
        }

        return new Scheduler(secondInterval, true, timeZone);
    }

    /// <summary>
    /// Creates a schedule for every second execution
    /// </summary>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    public static Scheduler EverySecond(TimeZoneInfo? timeZone = null)
    {
        return Secondly(1, timeZone);
    }

    /// <summary>
    /// Creates a schedule for daily execution at a specific time
    /// </summary>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when time parameters are out of valid ranges</exception>
    public static Scheduler DayAt(int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        ValidateTime(hour, minute, second);
        return Daily(new TimeOnly(hour, minute, second), timeZone);
    }

    /// <summary>
    /// Creates a schedule for weekly execution on a specific day at a specific time
    /// </summary>
    /// <param name="dayOfWeek">Day of week</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when time parameters are out of valid ranges</exception>
    public static Scheduler WeekAt(DayOfWeek dayOfWeek, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        ValidateTime(hour, minute, second);
        return Weekly(dayOfWeek, new TimeOnly(hour, minute, second), timeZone);
    }

    /// <summary>
    /// Creates a schedule for weekly execution on multiple days at a specific time
    /// </summary>
    /// <param name="daysOfWeek">Days of week</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when time parameters are out of valid ranges</exception>
    /// <exception cref="ArgumentException">Thrown when days of week are empty</exception>
    public static Scheduler WeekAt(IEnumerable<DayOfWeek> daysOfWeek, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        ValidateTime(hour, minute, second);
        return Weekly(daysOfWeek, new TimeOnly(hour, minute, second), timeZone);
    }

    /// <summary>
    /// Creates a schedule for monthly execution on a specific day at a specific time
    /// </summary>
    /// <param name="dayOfMonth">Day of month (1-31)</param>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <param name="timeZone">Time zone to use (defaults to UTC)</param>
    /// <returns>Schedule object</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are out of valid ranges</exception>
    public static Scheduler MonthAt(int dayOfMonth, int hour, int minute = 0, int second = 0, TimeZoneInfo? timeZone = null)
    {
        if (dayOfMonth < 1 || dayOfMonth > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Day of month must be between 1 and 31");
        }

        ValidateTime(hour, minute, second);
        return Monthly(dayOfMonth, new TimeOnly(hour, minute, second), timeZone);
    }

    #endregion

    /// <summary>
    /// Calculates the next execution time according to the schedule
    /// </summary>
    /// <returns>Date and time of the next execution</returns>
    /// <exception cref="InvalidOperationException">Thrown when the schedule type is not supported</exception>
    public virtual DateTime CalculateNextRunTime()
    {
        // Get the current time in the specified time zone
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);

        return Type switch
        {
            ScheduleType.Secondly => CalculateNextSecondlyRunTime(now),
            ScheduleType.Minutely => CalculateNextMinutelyRunTime(now),
            ScheduleType.Hourly => CalculateNextHourlyRunTime(now),
            ScheduleType.Daily => CalculateNextDailyRunTime(now),
            ScheduleType.Weekly => CalculateNextWeeklyRunTime(now),
            ScheduleType.Monthly => CalculateNextMonthlyRunTime(now),
            _ => throw new InvalidOperationException($"Unsupported schedule type: {Type}")
        };
    }

    /// <summary>
    /// Calculates the next execution time for secondly schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextSecondlyRunTime(DateTime now)
    {
        if (!SecondInterval.HasValue)
        {
            throw new InvalidOperationException("Second interval is not set for secondly schedule");
        }

        // Round current second down to nearest multiple of interval
        int currentSecond = now.Second;
        int currentMillisecond = now.Millisecond;

        int secondInterval = SecondInterval.Value;
        int nearestMultiple = currentSecond / secondInterval * secondInterval;

        // Calculate the next execution time
        DateTime nextRun = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            now.Hour,
            now.Minute,
            nearestMultiple,
            0,
            now.Kind);

        // If we're already past the nearest multiple, add the interval
        if (nearestMultiple < currentSecond ||
            nearestMultiple == currentSecond && currentMillisecond > 0)
        {
            nextRun = nextRun.AddSeconds(secondInterval);
        }

        // Check if we need to roll over to the next minute
        if (nextRun.Second >= 60)
        {
            nextRun = nextRun.AddMinutes(1);
            nextRun = new DateTime(
                nextRun.Year,
                nextRun.Month,
                nextRun.Day,
                nextRun.Hour,
                nextRun.Minute,
                nextRun.Second % 60,
                0,
                nextRun.Kind);
        }

        return nextRun;
    }

    /// <summary>
    /// Calculates the next execution time for minutely schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextMinutelyRunTime(DateTime now)
    {
        if (!MinuteInterval.HasValue)
        {
            throw new InvalidOperationException("Minute interval is not set for minutely schedule");
        }

        // Round current minute down to nearest multiple of interval
        int currentMinute = now.Minute;
        int currentSecond = now.Second;
        int currentMillisecond = now.Millisecond;

        int minuteInterval = MinuteInterval.Value;
        int nearestMultiple = currentMinute / minuteInterval * minuteInterval;

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
            nearestMultiple == currentMinute && (currentSecond > 0 || currentMillisecond > 0))
        {
            nextRun = nextRun.AddMinutes(minuteInterval);
        }

        // Check if we need to roll over to the next hour
        if (nextRun.Minute >= 60)
        {
            nextRun = nextRun.AddHours(1);
            nextRun = new DateTime(
                nextRun.Year,
                nextRun.Month,
                nextRun.Day,
                nextRun.Hour,
                nextRun.Minute % 60,
                0,
                0,
                nextRun.Kind);
        }

        return nextRun;
    }

    /// <summary>
    /// Calculates the next execution time for hourly schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextHourlyRunTime(DateTime now)
    {
        if (!HourInterval.HasValue || !MinuteOfHour.HasValue)
        {
            throw new InvalidOperationException("Hour interval or minute of hour is not set for hourly schedule");
        }

        int hourInterval = HourInterval.Value;
        int minuteOfHour = MinuteOfHour.Value;

        // Round current hour down to nearest multiple of interval
        int currentHour = now.Hour;
        int nearestHourMultiple = currentHour / hourInterval * hourInterval;

        // Create a scheduled time for the current interval
        DateTime scheduledTime = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            nearestHourMultiple,
            minuteOfHour,
            0,
            0,
            now.Kind);

        // If the scheduled time has already passed, go to the next interval
        if (scheduledTime <= now)
        {
            scheduledTime = scheduledTime.AddHours(hourInterval);

            // Check if we need to roll over to the next day
            if (scheduledTime.Hour >= 24)
            {
                scheduledTime = scheduledTime.AddDays(1);
                scheduledTime = new DateTime(
                    scheduledTime.Year,
                    scheduledTime.Month,
                    scheduledTime.Day,
                    scheduledTime.Hour % 24,
                    minuteOfHour,
                    0,
                    0,
                    scheduledTime.Kind);
            }
        }

        return scheduledTime;
    }

    /// <summary>
    /// Calculates the next execution time for daily schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextDailyRunTime(DateTime now)
    {
        var scheduledTime = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // If the scheduled time has already passed today, schedule for tomorrow
        if (now > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        return scheduledTime;
    }

    /// <summary>
    /// Calculates the next execution time for weekly schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextWeeklyRunTime(DateTime now)
    {
        if (DaysOfWeek != null && DaysOfWeek.Any())
        {
            return CalculateNextMultiDayWeeklyRunTime(now);
        }

        // Get the day of the week (or throw an exception if not set)
        var targetDayOfWeek = DayOfWeek ??
            throw new InvalidOperationException("Day of week is not set for weekly schedule");

        // Create a scheduled time for today
        var scheduledTime = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // Calculate days until the target day
        int daysUntilTargetDay = ((int)targetDayOfWeek - (int)now.DayOfWeek + 7) % 7;

        // If today is the target day but the time has already passed, go to next week
        if (daysUntilTargetDay == 0 && now > scheduledTime)
        {
            daysUntilTargetDay = 7;
        }

        return scheduledTime.AddDays(daysUntilTargetDay);
    }

    /// <summary>
    /// Calculates the next execution time for weekly schedule with multiple days
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    private DateTime CalculateNextMultiDayWeeklyRunTime(DateTime now)
    {
        if (DaysOfWeek == null || !DaysOfWeek.Any())
        {
            throw new InvalidOperationException("Days of week are not set for multi-day weekly schedule");
        }

        // Create a time for today at the scheduled time
        var scheduledTime = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // Check if today is one of the scheduled days and the time hasn't passed yet
        if (DaysOfWeek.Contains(now.DayOfWeek) && now <= scheduledTime)
        {
            return scheduledTime;
        }

        // Find the next scheduled day
        int minDaysToAdd = 7;
        foreach (var day in DaysOfWeek)
        {
            int daysUntilThisDay = ((int)day - (int)now.DayOfWeek + 7) % 7;

            // If today is one of the scheduled days but time has passed, we need the next occurrence (1 week later)
            if (daysUntilThisDay == 0)
            {
                daysUntilThisDay = 7;
            }

            minDaysToAdd = Math.Min(minDaysToAdd, daysUntilThisDay);
        }

        return scheduledTime.AddDays(minDaysToAdd);
    }

    /// <summary>
    /// Calculates the next execution time for monthly schedule
    /// </summary>
    /// <param name="now">Current date and time</param>
    /// <returns>Date and time of the next execution</returns>
    /// <exception cref="InvalidOperationException">Thrown when day of month is not set</exception>
    private DateTime CalculateNextMonthlyRunTime(DateTime now)
    {
        // Get the day of the month (or throw an exception if not set)
        var targetDayOfMonth = DayOfMonth ??
            throw new InvalidOperationException("Day of month is not set for monthly schedule");

        // Check if it's set to the last day of the month (31st)
        bool isLastDayOfMonth = targetDayOfMonth == 31;

        // Get the actual day for the current month (accounting for months with fewer days)
        int daysInCurrentMonth = DateTime.DaysInMonth(now.Year, now.Month);
        int actualDay = isLastDayOfMonth ? daysInCurrentMonth : Math.Min(targetDayOfMonth, daysInCurrentMonth);

        // Create scheduled time for the current month
        var scheduledTime = new DateTime(
            now.Year,
            now.Month,
            actualDay,
            TimeOfDay.Hour,
            TimeOfDay.Minute,
            TimeOfDay.Second,
            now.Kind);

        // If this date has already passed, move to the next month
        if (now > scheduledTime)
        {
            // Calculate for next month
            DateTime nextMonth = now.AddMonths(1);
            int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
            int nextMonthActualDay = isLastDayOfMonth ? daysInNextMonth : Math.Min(targetDayOfMonth, daysInNextMonth);

            scheduledTime = new DateTime(
                nextMonth.Year,
                nextMonth.Month,
                nextMonthActualDay,
                TimeOfDay.Hour,
                TimeOfDay.Minute,
                TimeOfDay.Second,
                now.Kind);
        }

        return scheduledTime;
    }

    /// <summary>
    /// Validates time values
    /// </summary>
    /// <param name="hour">Hour (0-23)</param>
    /// <param name="minute">Minute (0-59)</param>
    /// <param name="second">Second (0-59)</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when time values are out of valid ranges</exception>
    public static void ValidateTime(int hour, int minute, int second)
    {
        if (hour < 0 || hour > 23)
            throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 0 and 23");

        if (minute < 0 || minute > 59)
            throw new ArgumentOutOfRangeException(nameof(minute), "Minute must be between 0 and 59");

        if (second < 0 || second > 59)
            throw new ArgumentOutOfRangeException(nameof(second), "Second must be between 0 and 59");
    }

    /// <summary>
    /// Returns a string representation of the schedule
    /// </summary>
    /// <returns>Human-readable description of the schedule</returns>
    public override string ToString()
    {
        string timeZoneDisplay = TimeZone.Id == TimeZoneInfo.Utc.Id ? " UTC" : $" {TimeZone.DisplayName}";

        return Type switch
        {
            ScheduleType.Secondly => SecondInterval == 1
                ? $"Every second{timeZoneDisplay}"
                : $"Every {SecondInterval} seconds{timeZoneDisplay}",

            ScheduleType.Minutely => MinuteInterval == 1
                ? $"Every minute{timeZoneDisplay}"
                : $"Every {MinuteInterval} minutes{timeZoneDisplay}",

            ScheduleType.Hourly => HourInterval == 1
                ? $"Every hour at {MinuteOfHour} minutes past the hour{timeZoneDisplay}"
                : $"Every {HourInterval} hours at {MinuteOfHour} minutes past the hour{timeZoneDisplay}",

            ScheduleType.Daily => $"Daily at {TimeOfDay}{timeZoneDisplay}",

            ScheduleType.Weekly when DaysOfWeek != null && DaysOfWeek.Any() =>
                $"Weekly on {string.Join(", ", DaysOfWeek.Select(d => d.ToString()))} at {TimeOfDay}{timeZoneDisplay}",

            ScheduleType.Weekly => $"Weekly on {DayOfWeek} at {TimeOfDay}{timeZoneDisplay}",

            ScheduleType.Monthly => DayOfMonth == 31
                ? $"On the last day of every month at {TimeOfDay}{timeZoneDisplay}"
                : $"On the {DayOfMonth}{GetOrdinalSuffix(DayOfMonth.Value)} day of every month at {TimeOfDay}{timeZoneDisplay}",

            _ => throw new ArgumentException($"Unsupported schedule type: {Type}")
        };
    }

    /// <summary>
    /// Gets the ordinal suffix for a number (1st, 2nd, 3rd, etc.)
    /// </summary>
    /// <param name="number">The number to get suffix for</param>
    /// <returns>The ordinal suffix</returns>
    private static string GetOrdinalSuffix(int number)
    {
        int remainder = number % 100;
        if (remainder >= 11 && remainder <= 13)
            return "th";

        switch (number % 10)
        {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }

    #region IEquatable implementation

    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Scheduler other && Equals(other);
    }

    /// <summary>
    /// Determines whether the specified scheduler is equal to the current scheduler
    /// </summary>
    /// <param name="other">The scheduler to compare with the current scheduler</param>
    /// <returns>true if the specified scheduler is equal to the current scheduler; otherwise, false</returns>
    public bool Equals(Scheduler? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Type == other.Type &&
               TimeOfDay.Equals(other.TimeOfDay) &&
               DayOfWeek == other.DayOfWeek &&
               (DaysOfWeek == null && other.DaysOfWeek == null ||
                DaysOfWeek != null && other.DaysOfWeek != null && DaysOfWeek.SetEquals(other.DaysOfWeek)) &&
               DayOfMonth == other.DayOfMonth &&
               TimeZone.Id == other.TimeZone.Id &&
               SecondInterval == other.SecondInterval &&
               MinuteInterval == other.MinuteInterval &&
               HourInterval == other.HourInterval &&
               MinuteOfHour == other.MinuteOfHour;
    }

    /// <summary>
    /// Serves as the default hash function
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add((int)Type);
        hash.Add(TimeOfDay);
        hash.Add(DayOfWeek);

        if (DaysOfWeek != null)
        {
            foreach (var day in DaysOfWeek.OrderBy(d => d))
            {
                hash.Add(day);
            }
        }

        hash.Add(DayOfMonth);
        hash.Add(TimeZone.Id);
        hash.Add(SecondInterval);
        hash.Add(MinuteInterval);
        hash.Add(HourInterval);
        hash.Add(MinuteOfHour);

        return hash.ToHashCode();
    }

    /// <summary>
    /// Determines whether two schedulers are equal
    /// </summary>
    /// <param name="left">The first scheduler</param>
    /// <param name="right">The second scheduler</param>
    /// <returns>true if the schedulers are equal; otherwise, false</returns>
    public static bool operator ==(Scheduler? left, Scheduler? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two schedulers are not equal
    /// </summary>
    /// <param name="left">The first scheduler</param>
    /// <param name="right">The second scheduler</param>
    /// <returns>true if the schedulers are not equal; otherwise, false</returns>
    public static bool operator !=(Scheduler? left, Scheduler? right)
    {
        return !(left == right);
    }

    #endregion

    #region Fluent Interface Extensions

    /// <summary>
    /// Configures the scheduler to use the specified time zone
    /// </summary>
    /// <param name="timeZone">The time zone to use</param>
    /// <returns>A new scheduler with the specified time zone</returns>
    public Scheduler InTimeZone(TimeZoneInfo timeZone)
    {
        return Type switch
        {
            ScheduleType.Secondly => new Scheduler(SecondInterval!.Value, true, timeZone),
            ScheduleType.Minutely => new Scheduler(MinuteInterval!.Value, timeZone),
            ScheduleType.Hourly => new Scheduler(HourInterval!.Value, MinuteOfHour!.Value, timeZone),
            ScheduleType.Daily => new Scheduler(TimeOfDay, timeZone),
            ScheduleType.Weekly when DaysOfWeek != null => new Scheduler(DaysOfWeek, TimeOfDay, timeZone),
            ScheduleType.Weekly => new Scheduler(DayOfWeek!.Value, TimeOfDay, timeZone),
            ScheduleType.Monthly => new Scheduler(DayOfMonth!.Value, TimeOfDay, timeZone),
            _ => throw new InvalidOperationException($"Unsupported schedule type: {Type}")
        };
    }

    /// <summary>
    /// Configures the scheduler to use local time zone
    /// </summary>
    /// <returns>A new scheduler using local time zone</returns>
    public Scheduler InLocalTime()
    {
        return InTimeZone(TimeZoneInfo.Local);
    }

    /// <summary>
    /// Configures the scheduler to use UTC time zone
    /// </summary>
    /// <returns>A new scheduler using UTC time zone</returns>
    public Scheduler InUtc()
    {
        return InTimeZone(TimeZoneInfo.Utc);
    }

    #endregion
}