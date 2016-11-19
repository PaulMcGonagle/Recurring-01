using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : ISchedule
    {

        public LocalDate? DateFrom;
        public LocalDate? DateTo;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }
        protected int Increment = 1;

        public abstract IEnumerable<LocalDate> Dates { get; }
    }
}
