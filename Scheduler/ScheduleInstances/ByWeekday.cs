using System;
using System.Collections.Generic;
using NodaTime;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekday : ScheduleAbstracts.Repeating
    {
        [DataMember]
        public IsoDayOfWeek Weekday;

        public ByWeekday(
            IsoDayOfWeek weekday)
        {
            Weekday = weekday;

            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public static ByWeekday Create(
            IsoDayOfWeek isoDayOfWeek,
            IRangeDate rangeDate)
        {
            var byWeekday = new ByWeekday(
                weekday: isoDayOfWeek)
            {
                EdgeRange = new EdgeRangeDate(rangeDate),
            };

            return byWeekday;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            EdgeRange?.Range.Validate();

            LocalDate startDate;

            if (EdgeRange?.Range?.Start != null)
            {
                startDate = DateTimeHelper.GetNextWeekday(EdgeRange.Range.Start.Date.Value, Weekday);
            }
            else
            {
                startDate =
                    DateTimeHelper.GetNextWeekday(clock.GetLocalDateTime().Date, Weekday)
                        .PlusWeeks(CountFrom ?? CountFromDefault);
            }

            int weeks;

            if (EdgeRange?.Range?.End != null)
            {
                var weeksLong = Period.Between(startDate, EdgeRange.Range.End.Date.Value, PeriodUnits.Weeks).Weeks;

                if (weeksLong > int.MaxValue || weeksLong < 0)
                    throw new ArgumentException("Period is out of scope for Int {weeksLong}");

                weeks = (int) weeksLong;
            }
            else
            {
                weeks = CountTo ?? CountToDefault;
            }

            var results = new List<IDate>
            {
                new Date(startDate),
            };

            var s = Enumerable.Range(1, weeks);

            results.AddRange(
                s.Select(o => new Date(startDate.PlusWeeks(o))));

            results.Sort();

            return results;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByWeekday>(db);
            base.Save(db, clock);
        }
    }
}
