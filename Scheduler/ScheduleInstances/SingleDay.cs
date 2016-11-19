using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : ISchedule
    {
        public SingleDay()
        {
        }

        public LocalDate Date
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Dates
        {
            get { yield return Date; }
        }        
    }
}
