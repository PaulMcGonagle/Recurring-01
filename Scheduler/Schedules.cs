using System.Collections.Generic;
using System.Linq;
using Scheduler.Generation;

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

        public IEnumerable<GeneratedDate> GenerateDates()
        {
            return this.SelectMany(d => d.GenerateDates());
        }
    }
}
