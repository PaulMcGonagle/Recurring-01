using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Episode : PersistableEntity, IComparable
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
            return From.CompareTo(c.From);
        }

        public override string ToString()
        {
            return From.ToString() + " " + To.ToString();
        }

        public void Save(IArangoDatabase db)
        {
            Save<Episode>(db);
        }
    }
}
