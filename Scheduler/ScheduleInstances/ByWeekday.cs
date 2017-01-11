using System;
using NodaTime;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekday : ScheduleAbstracts.Repeating
    {
        [DataMember]
        public IsoDayOfWeek Weekday;
        private IClock _clock;

        public ByWeekday(IClock clock, IsoDayOfWeek weekday)
        {
            _clock = clock;
            Weekday = weekday;

            CountFromDefault = 0;
            CountToDefault = 52;
        }

        [IgnoreDataMember]
        public IClock Clock
        {
            // ToDo instantiate _clock using IOC
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        public static ByWeekday Create(
            IClock clock,
            IsoDayOfWeek weekday,
            Range range)
        {
            var byWeekday = new ByWeekday(
                clock: clock,
                weekday: weekday)
            {
                EdgeRange = new EdgeRange(range),
            };

            return byWeekday;
        }

        public override IEnumerable<Scheduler.Date> GenerateDates()
        {
            if (Clock == null)
                throw new ArgumentNullException($"Clock");

            EdgeRange?.Range.Validate();

            LocalDate startDate;

            if (EdgeRange?.Range?.From != null)
            {
                startDate = DateTimeHelper.GetNextWeekday(EdgeRange.Range.From.Value, Weekday);
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
                var weeksLong = Period.Between(startDate, EdgeRange.Range.To.Value, PeriodUnits.Weeks).Weeks;

                if (weeksLong > int.MaxValue || weeksLong < 0)
                    throw new ArgumentException("Period is out of scope for Int {weeksLong}");

                weeks = (int) weeksLong;
            }
            else
            {
                weeks = CountTo ?? CountToDefault;
            }

            var results = new List<Scheduler.Date>
            {
                new Scheduler.Date(startDate)
            };

            var s = Enumerable.Range(1, weeks);
            results.AddRange(s.Select(o => new Scheduler.Date(startDate.PlusWeeks(o))));

            results.Sort();

            return results;
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<ByWeekday>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
