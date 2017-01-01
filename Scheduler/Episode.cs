using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Episode : Vertex, IComparable
    {
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

        public override string ToString()
        {
            return From.ToString() + " " + To.ToString();
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Episode>(db),
                () => base.Save(db),
            });
        }
    }
}
