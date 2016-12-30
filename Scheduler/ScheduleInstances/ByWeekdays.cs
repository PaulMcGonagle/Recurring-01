using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Days;

        [IgnoreDataMember]
        public IClock Clock { get; set; }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                var start = Range.From ?? DateTimeHelper.GetToday(Clock).AddWeeks(-(CountFrom ?? CountFromDefault));
                var end = Range.To ?? DateTimeHelper.GetToday(Clock).AddWeeks((CountTo ?? CountToDefault));

                var range = DateTimeHelper.Range(start: start, end: end);
                return range.Where(d => Days.Contains(d.IsoDayOfWeek));
            }
        }
    }
}
