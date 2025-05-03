namespace HostedService.Extensions;

/// <summary>
/// Provides a way to mock the current time for testing
/// </summary>
public static class SystemTime
{
    private static DateTime? _fixedPointTime;
    private static TimeSpan? _offsetFromUtcNow;

    /// <summary>
    /// Gets the current time, either the mocked time offset or the real system time
    /// </summary>
    public static DateTime Now
    {
        get
        {
            if (_offsetFromUtcNow.HasValue)
            {
                return DateTime.UtcNow.Add(_offsetFromUtcNow.Value);
            }
            return _fixedPointTime ?? DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Sets the current time for testing
    /// </summary>
    /// <param name="time">The time to set</param>
    public static void SetCurrentTime(DateTime time)
    {
        _offsetFromUtcNow = time - DateTime.UtcNow;
        _fixedPointTime = null;
    }

    /// <summary>
    /// Sets a fixed time that won't change
    /// </summary>
    /// <param name="time">The fixed time to set</param>
    public static void SetFixedTime(DateTime time)
    {
        _fixedPointTime = time;
        _offsetFromUtcNow = null;
    }

    /// <summary>
    /// Resets the current time to use the real system time
    /// </summary>
    public static void Reset()
    {
        _fixedPointTime = null;
        _offsetFromUtcNow = null;
    }
}