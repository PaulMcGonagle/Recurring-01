using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Episode : PersitableEntity, IComparable
    {
        public ZonedDateTime From;
        public Period Period;
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
