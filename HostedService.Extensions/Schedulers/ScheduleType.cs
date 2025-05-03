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