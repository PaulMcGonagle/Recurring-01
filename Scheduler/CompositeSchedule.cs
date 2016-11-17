using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Scheduler
{
    public class CompositeSchedule : ISchedule
    {
        public List<ISchedule> Inclusions = new Schedules();
        public List<ISchedule> Exclusions = new Schedules();

        public CompositeSchedule()
        {

        }

        public IEnumerable<LocalDate> Dates()
        {
            var inclusions = Inclusions.SelectMany(i => i.Dates());

            var exclusions = Exclusions.SelectMany(e => e.Dates());

            return inclusions.Except(exclusions);
        }
    }
}
