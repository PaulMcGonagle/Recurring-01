using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Scheduler
{
    public class CompositeSchedule : ScheduleBase
    {
        public List<ScheduleBase> Inclusions = new Schedules();
        public List<ScheduleBase> Exclusions = new Schedules();

        public CompositeSchedule()
        {

        }

        public override IEnumerable<LocalDate> Occurrences()
        {
            var inclusions = Inclusions.SelectMany(i => i.Occurrences());

            var exclusions = Exclusions.SelectMany(e => e.Occurrences());

            return inclusions.Except(exclusions);
        }
    }
}
