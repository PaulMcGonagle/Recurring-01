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
        private IClock _clock;

        public ByWeekday(
            IsoDayOfWeek weekday, 
            IClock clock = null)
        {
            Weekday = weekday;
            Clock = clock;

            CountFromDefault = 0;
            CountToDefault = 52;
        }

        [IgnoreDataMember]
        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }
            set { _clock = value; }
        }

        public static ByWeekday Create(
            IsoDayOfWeek isoDayOfWeek,
            DateRange dateRange,
            IClock clock = null)
        {
            var byWeekday = new ByWeekday(
                weekday: isoDayOfWeek,
                clock: clock)
            {
                EdgeRange = new EdgeRangeDate(dateRange),
            };

            return byWeekday;
        }

        public override IEnumerable<IDate> Generate()
        {
            EdgeRange?.Range.Validate();

            LocalDate startDate;

            if (EdgeRange?.Range?.From != null)
            {
                startDate = DateTimeHelper.GetNextWeekday(EdgeRange.Range.From.Date.Value, Weekday);
            }
            else
            {
                startDate =
                    DateTimeHelper.GetNextWeekday(Clock.GetLocalDateTime().Date, Weekday)
                        .PlusWeeks(CountFrom ?? CountFromDefault);
            }

            int weeks;

            if (EdgeRange?.Range?.To != null)
            {
                var weeksLong = Period.Between(startDate, EdgeRange.Range.To.Date.Value, PeriodUnits.Weeks).Weeks;

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
