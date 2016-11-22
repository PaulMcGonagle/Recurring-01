using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Scheduler
{
    public class CompositeSchedule : ISchedule
    {
        public List<ISchedule> Inclusions = new Schedules();
        public List<ISchedule> Exclusions = new Schedules();

        public List<Range> Breaks = new List<Range>();

        public CompositeSchedule()
        {

        }

        public IEnumerable<LocalDate> Dates
        {
            get
            {
                var inclusions = Inclusions.SelectMany(i => i.Dates);
                var exclusions = Exclusions.SelectMany(e => e.Dates);

                var list = inclusions.Exclude(exclusions);

                list = list.Exclude(Breaks);

                return list;
            }
        }
    }
}
