using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : ScheduleBase
    {
        public SingleDay()
        {
        }

        public LocalDate Date
        {
            get;
            set;
        }

        public override IEnumerable<LocalDate> Occurrences()
        {
            yield return Date;
        }        
    }
}
