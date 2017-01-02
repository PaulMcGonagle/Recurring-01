using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfYear : ScheduleAbstracts.RepeatingDay
    {
        private int _dayOfYear = 1;
        private IClock _clock;

        public int DayOfYear
        {
            get { return _dayOfYear; }
            set
            {
                if (value < 1 || value > 31)
                {
                    throw new ArgumentOutOfRangeException($"Invalid DayOfYear value '{value}'");
                }

                _dayOfYear = value;
            }
        }

        [IgnoreDataMember]
        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        [IgnoreDataMember]
        public YearMonth.MonthValue Month { get; set; } = YearMonth.MonthValue.January;

        [IgnoreDataMember]
        protected int YearFrom
        {
            get
            {
                if (EdgeRange?.ToVertex?.From != null)
                    return new Date(EdgeRange.ToVertex.From.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountFrom ?? CountFromDefault).Year;
            }
        }

        [IgnoreDataMember]
        protected int YearTo
        {
            get
            {
                if (EdgeRange?.ToVertex?.To != null)
                    return new Date(EdgeRange.ToVertex.To.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountTo ?? CountToDefault).Year;
            }
        }

        public override IEnumerable<Scheduler.Date> GenerateDates()
        {
            foreach (var year in Enumerable.Range(YearFrom, YearTo - YearFrom + 1))
            {
                var yearMonth = new YearMonth {Year = year, Month = Month};

                yield return yearMonth.ToLocalDate(DayOfYear, RollStrategy);
            }
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<ByDayOfYear>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
