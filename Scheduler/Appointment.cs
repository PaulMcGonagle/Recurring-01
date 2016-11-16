using System;
using NodaTime;

namespace Scheduler
{
    public class Appointment : IComparable
    {
        public ZonedDateTime From;
        public Period Period;
        public ZonedDateTime End
        {
            get
            {
                return From.Plus(Period.ToDuration());
            }
        }

        int IComparable.CompareTo(object obj)
        {
            var c = (Appointment)obj;
            return this.From.CompareTo(c.From);
        }

        public override string ToString()
        {
            return From.ToString() + " " + End.ToString();
        }
    }
}
