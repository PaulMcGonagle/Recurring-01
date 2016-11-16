using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public class Schedules : List<ScheduleBase>
    {
        public Schedules()
        {

        }

        public Schedules(IEnumerable<ScheduleBase> occurrences)
        {
            this.AddRange(occurrences);
        }

        public Schedules(ScheduleBase occurrence)
        {
            this.Add(occurrence);
        }

        public IEnumerable<LocalDate> Occurrences
        {
            get
            {
                List<LocalDate> o = new List<LocalDate>();
                foreach (var i in this)
                    o.AddRange(i.Occurrences());

                return o.Distinct();
            }
        }
    }
}
