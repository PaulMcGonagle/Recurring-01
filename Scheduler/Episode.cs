using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

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
            get => _start;
            set
            {
                _start = value;
                SetDirty();
            }
        }

        private Period _period;

        public Period Period
        {
            get => _period;
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
            SourceGeneratedDate.Save(db, clock, this, "SourceGeneratedDate");
            SourceSerial.Save(db, clock, this, "SourceSerial");
            base.Save(db, clock);
        }

        public class Builder : Vertex.Builder<Episode>
        {
            public IDate SourceGeneratedDate
            {
                set => _target.SourceGeneratedDate = new EdgeDate(value);
            }

            public ISerial SourceSerial
            {
                set => _target.SourceSerial = new EdgeVertex<ISerial>(value);
            }

            public ZonedDateTime Start
            {
                set => _target.Start = value;
            }

            public Period Period
            {
                set => _target.Period = value;
            }
        }
    }
}
