namespace HostedService.Extensions.Schedulers;

/// <summary>
/// Factory methods for creating Scheduler instances
/// </summary>
public partial class Scheduler
{
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
    /// e.g. Scheduler.Hourly(2, 30); Every 2 hours at minute 30
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
}