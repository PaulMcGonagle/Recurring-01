using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Days;

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override IEnumerable<LocalDate> Occurrences()
        {
            LocalDate start = DateFrom ?? DateTimeHelper.GetToday().AddWeeks(-(CountFrom ?? CountFromDefault));
            LocalDate end = DateTo ?? DateTimeHelper.GetToday().AddWeeks((CountTo ?? CountToDefault));

            var range = DateTimeHelper.Range(start: start, end: end);
            return range.Where(d => Days.Contains(d.IsoDayOfWeek));
        }
    }
}
