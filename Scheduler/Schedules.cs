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

        public IEnumerable<IDate> GenerateDates()
        {
            var results = new List<IDate>();

            results.AddRange(this.SelectMany(d => d.Generate()));

            return results;
        }
    }
}
