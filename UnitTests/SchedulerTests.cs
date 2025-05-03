using HostedService.Extensions;
using HostedService.Extensions.Schedulers;

namespace UnitTests;

public class SchedulerTests : IDisposable
{
    private readonly TimeZoneInfo _testTimeZone = TimeZoneInfo.Utc;

    public SchedulerTests()
    {
        // Set a fixed time for all tests
        SystemTime.SetCurrentTime(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
    }

    public void Dispose()
    {
        // Reset SystemTime after each test
        SystemTime.Reset();
    }

    [Theory]
    [InlineData(15, 30, 0, "2024-01-01T15:30:00")] // Same day
    [InlineData(10, 0, 0, "2024-01-02T10:00:00")] // Next day
    [InlineData(23, 59, 59, "2024-01-01T23:59:59")] // Same day
    public void DailySchedule_ShouldCalculateNextRunTimeCorrectly(int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Daily(timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 10, 0, 0, "2024-01-08T10:00:00")] // Next Monday
    [InlineData(DayOfWeek.Wednesday, 9, 0, 0, "2024-01-03T09:00:00")] // Next Wednesday
    [InlineData(DayOfWeek.Friday, 15, 30, 0, "2024-01-05T15:30:00")] // Next Friday
    public void WeeklySchedule_ShouldCalculateNextRunTimeCorrectly(DayOfWeek dayOfWeek, int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Weekly(dayOfWeek, timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(15, 12, 0, 0, "2024-01-15T12:00:00")] // Same month
    [InlineData(31, 10, 0, 0, "2024-01-31T10:00:00")] // Last day of month
    [InlineData(1, 9, 0, 0, "2024-02-01T09:00:00")] // Next month
    public void MonthlySchedule_ShouldCalculateNextRunTimeCorrectly(int dayOfMonth, int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Monthly(dayOfMonth, timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(2, 30, "2024-01-01T12:30:00")] // Same hour
    [InlineData(1, 0, "2024-01-01T13:00:00")] // Next hour
    [InlineData(4, 45, "2024-01-01T12:45:00")] // Same hour
    public void HourlySchedule_ShouldCalculateNextRunTimeCorrectly(int hourInterval, int minuteOfHour, string expectedTimeStr)
    {
        // Arrange
        var scheduler = Scheduler.Hourly(hourInterval, minuteOfHour, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(15, "2024-01-01T12:15:00")] // Next 15 minutes
    [InlineData(30, "2024-01-01T12:30:00")] // Next 30 minutes
    [InlineData(5, "2024-01-01T12:05:00")] // Next 5 minutes
    public void MinutelySchedule_ShouldCalculateNextRunTimeCorrectly(int minuteInterval, string expectedTimeStr)
    {
        // Arrange
        var scheduler = Scheduler.Minutely(minuteInterval, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(30, "2024-01-01T12:00:30")] // Next 30 seconds
    [InlineData(15, "2024-01-01T12:00:15")] // Next 15 seconds
    [InlineData(5, "2024-01-01T12:00:05")] // Next 5 seconds
    public void SecondlySchedule_ShouldCalculateNextRunTimeCorrectly(int secondInterval, string expectedTimeStr)
    {
        // Arrange
        var scheduler = Scheduler.Secondly(secondInterval, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, 9, 0, 0, "2024-01-03T09:00:00")] // Next Wednesday
    [InlineData(new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 15, 30, 0, "2024-01-02T15:30:00")] // Next Tuesday
    [InlineData(new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }, 10, 0, 0, "2024-01-06T10:00:00")] // Next Saturday
    public void MultiDayWeeklySchedule_ShouldCalculateNextRunTimeCorrectly(DayOfWeek[] daysOfWeek, int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Weekly(daysOfWeek, timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(10, 0, 0, "2024-01-02T10:00:00")] // Next day
    [InlineData(15, 30, 0, "2024-01-01T15:30:00")] // Same day
    [InlineData(23, 59, 59, "2024-01-01T23:59:59")] // Same day
    public void ScheduleWithTimeZone_ShouldConvertTimesCorrectly(int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Daily(timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(23, 59, 59, "2024-01-01T23:59:59")] // Edge case: end of day
    [InlineData(0, 0, 0, "2024-01-02T00:00:00")] // Edge case: start of day
    [InlineData(12, 0, 0, "2024-01-02T12:00:00")] // Edge case: exactly current time
    public void Schedule_ShouldHandleEdgeCases(int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Daily(timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }

    [Theory]
    [InlineData(32, 10, 0, 0)] // Invalid day of month
    public void MonthlySchedule_ShouldHandleInvalidDayOfMonth(int dayOfMonth, int hour, int minute, int second)
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Scheduler.Monthly(dayOfMonth, new TimeOnly(hour, minute, second)));
    }

    [Theory]
    [InlineData(24, 0)] // Invalid hour interval
    public void HourlySchedule_ShouldHandleInvalidHourInterval(int hourInterval, int minuteOfHour)
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Scheduler.Hourly(hourInterval, minuteOfHour));
    }

    [Theory]
    [InlineData(60)] // Invalid minute interval
    public void MinutelySchedule_ShouldHandleInvalidMinuteInterval(int minuteInterval)
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Scheduler.Minutely(minuteInterval));
    }

    [Theory]
    [InlineData(60)] // Invalid second interval
    public void SecondlySchedule_ShouldHandleInvalidSecondInterval(int secondInterval)
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Scheduler.Secondly(secondInterval));
    }

    [Fact]
    public void Schedule_ShouldHandleEmptyDaysOfWeek()
    {
        // Assert
        Assert.Throws<ArgumentException>(() => Scheduler.Weekly(Array.Empty<DayOfWeek>(), new TimeOnly(10, 0, 0)));
    }

    [Theory]
    [InlineData(31, 10, 0, 0, "2024-01-31T10:00:00")] // January has 31 days
    [InlineData(30, 9, 0, 0, "2024-01-30T09:00:00")] // January has 30 days
    [InlineData(29, 15, 30, 0, "2024-01-29T15:30:00")] // January has 29 days
    public void Schedule_ShouldHandleLastDayOfMonth(int dayOfMonth, int hour, int minute, int second, string expectedTimeStr)
    {
        // Arrange
        var timeOfDay = new TimeOnly(hour, minute, second);
        var scheduler = Scheduler.Monthly(dayOfMonth, timeOfDay, _testTimeZone);
        var expectedNextRun = DateTime.Parse(expectedTimeStr);

        // Act
        var nextRun = scheduler.CalculateNextRunTime();

        // Assert
        Assert.Equal(expectedNextRun, nextRun, TimeSpan.FromMilliseconds(5));
    }
}