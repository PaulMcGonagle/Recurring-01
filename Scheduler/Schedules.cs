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

        public IEnumerable<Date> GenerateDates()
        {
            return this.SelectMany(d => d.GenerateDates());
        }
    }
}
