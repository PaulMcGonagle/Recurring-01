using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Episode : Vertex, IEpisode
    {
        [IgnoreDataMember]
        public EdgeVertex<IDate> SourceGeneratedDate { get; set; }

        [IgnoreDataMember]
        public EdgeVertex<ISerial> SourceSerial { get; set; }

        private ZonedDateTime _start;

        public ZonedDateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                SetDirty();
            }
        }

        private Period _period;

        public Period Period
        {
            get { return _period; }
            set
            {
                _period = value;
                SetDirty();
            }
        }

        [IgnoreDataMember]
        public ZonedDateTime To => Start.Plus(Period.ToDuration());

        int IComparable.CompareTo(object obj)
        {
            var c = (Episode)obj;

            var fromCompare = Start.CompareTo(c.Start);

            if (fromCompare != 0)
                return fromCompare;

            return Period.Ticks.CompareTo(c.Period.Ticks);
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Episode>(db);
            base.Save(db, clock);
        }
    }
}
