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

        public IEnumerable<LocalDate> Occurrences()
        {
            var inclusions = Inclusions.SelectMany(i => i.Occurrences());

            var exclusions = Exclusions.SelectMany(e => e.Occurrences());

            return inclusions.Except(exclusions);
        }
    }
}
