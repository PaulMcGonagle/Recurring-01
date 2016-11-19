using System;
using NodaTime;

namespace Scheduler
{
    public class Episode : IComparable
    {
        public ZonedDateTime From;
        public Period Period;
        public ZonedDateTime End => From.Plus(Period.ToDuration());

        int IComparable.CompareTo(object obj)
        {
            var c = (Episode)obj;
            return From.CompareTo(c.From);
        }

        public override string ToString()
        {
            return From.ToString() + " " + End.ToString();
        }
    }
}
