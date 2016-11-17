using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class DateList : ISchedule
    {
        public DateList()
        {
        }

        public IEnumerable<LocalDate> Dates
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Occurrences()
        {
            return Dates;
        }
    }
}
