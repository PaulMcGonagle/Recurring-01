using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public class TimeRange : Vertex, ITimeRange
    {
        public LocalTime From { get; set; }

        public Period Period { get; set; }

        public TimeRange(LocalTime from, Period period)
        {
            From = from;
            Period = period;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<TimeRange>(db);
            base.Save(db, clock);
        }
    }
}
