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

        public Schedules(ISchedule schedule)
        {
            Add(schedule);
        }

        public IEnumerable<LocalDate> Dates()
        {
            return this.SelectMany(d => d.Dates);
        }
    }
}
