using NodaTime;

namespace Scheduler.Ranges
{
    public interface ITimeRange
    {
        LocalTime From { get; set; }
        Period Period { get; set; }
    }
}