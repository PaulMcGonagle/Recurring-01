using System.Collections.Generic;
using System.Linq;
using NodaTime;

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

        public IEnumerable<IDate> GenerateDates(IClock clock)
        {
            var results = new List<IDate>();

            results.AddRange(this.SelectMany(d => d.Generate(clock)));

            return results;
        }
    }
}
