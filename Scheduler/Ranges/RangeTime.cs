using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public class RangeTime : Vertex, IRangeTime
    {
        public LocalTime Start { get; set; }

        public Period Period { get; set; }

        [IgnoreDataMember]
        public LocalTime End => Start.Plus(Period);

        public RangeTime(LocalTime start, Period period)
        {
            Start = start;
            Period = period;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeTime>(db);
            base.Save(db, clock);
        }
    }
}
