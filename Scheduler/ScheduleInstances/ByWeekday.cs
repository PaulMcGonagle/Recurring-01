using NodaTime;
using System.Linq;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekday : ScheduleAbstracts.Repeating
    {
        public IsoDayOfWeek Weekday;
        public IClock Clock;

        public ByWeekday()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override IEnumerable<LocalDate> Dates
        {
            get
            {
                var localDateTime = Clock.GetLocalDateTime();
                var localDate = localDateTime.Date;
                var localDay = localDate.IsoDayOfWeek;

                var offset = localDay < Weekday ? localDay - Weekday + 7 : localDay - Weekday;

                var startDay = localDate.PlusDays(-offset);

                var results = new List<LocalDate>
                {
                    startDay
                };

                var r = Enumerable.Range(1, CountFromDefault);
                results.AddRange(r.Select(o => startDay.PlusWeeks(-o)));
                var s = Enumerable.Range(1, CountToDefault);
                results.AddRange(s.Select(o => startDay.PlusWeeks(o)));

                results.Sort();

                return results;
            }
        }
    }
}
