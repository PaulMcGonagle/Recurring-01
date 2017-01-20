using NodaTime;

namespace Scheduler.Ranges
{
    public class TimeRange : ITimeRange
    {
        public LocalTime From { get; set; }

        public Period Period { get; set; }

        public TimeRange(LocalTime from, Period period)
        {
            From = from;
            Period = period;
        }
    }
}
