using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public class RangeTime : Vertex, IRangeTime
    {
        public LocalTime From { get; set; }

        public Period Period { get; set; }

        public RangeTime(LocalTime from, Period period)
        {
            From = from;
            Period = period;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeTime>(db);
            base.Save(db, clock);
        }
    }
}
