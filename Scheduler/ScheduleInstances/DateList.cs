using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class DateList : ScheduleBase
    {
        public DateList()
        {
        }

        public IEnumerable<LocalDate> Dates
        {
            get;
            set;
        }

        public override IEnumerable<LocalDate> Occurrences()
        {
            return Dates;
        }
    }
}
