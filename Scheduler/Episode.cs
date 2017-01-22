using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Episode : Vertex, IComparable, IEpisode
    {
        [IgnoreDataMember]
        public EdgeVertex<IGeneratedDate> SourceGeneratedDate { get; set; }

        [IgnoreDataMember]
        public EdgeVertex<ISerial> SourceSerial { get; set; }

        private ZonedDateTime _from;

        public ZonedDateTime From
        {
            get { return _from; }
            set
            {
                _from = value;
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
        public ZonedDateTime To => From.Plus(Period.ToDuration());

        int IComparable.CompareTo(object obj)
        {
            var c = (Episode)obj;

            var fromCompare = From.CompareTo(c.From);

            if (fromCompare != 0)
                return fromCompare;

            return Period.Ticks.CompareTo(c.Period.Ticks);
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Episode>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
