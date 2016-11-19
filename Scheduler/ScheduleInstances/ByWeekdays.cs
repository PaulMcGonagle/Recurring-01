using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Days;
        public IClock Clock { get; set; }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override IEnumerable<LocalDate> Dates()
        {
            LocalDate start = DateFrom ?? DateTimeHelper.GetToday(Clock).AddWeeks(-(CountFrom ?? CountFromDefault));
            LocalDate end = DateTo ?? DateTimeHelper.GetToday(Clock).AddWeeks((CountTo ?? CountToDefault));

            var range = DateTimeHelper.Range(start: start, end: end);
            return range.Where(d => Days.Contains(d.IsoDayOfWeek));
        }
    }
}
