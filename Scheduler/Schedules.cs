using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public class Schedules : List<ISchedule>
    {
        public Schedules()
        {

        }

        public Schedules(ISchedule occurrence)
        {
            this.Add(occurrence);
        }

        public IEnumerable<LocalDate> Occurrences()
        {
            return this.SelectMany(o => o.Dates());
        }
    }
}
