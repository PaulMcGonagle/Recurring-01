using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        public YearMonth.MonthValue Month { get; set; } = YearMonth.MonthValue.January;

        protected int YearFrom
        {
            get
            {
                if (Range?.From != null)
                    return new Scheduler.Date(Range.From.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountFrom ?? CountFromDefault).Year;
            }
        }

        protected int YearTo
        {
            get
            {
                if (Range?.To != null)
                    return new Scheduler.Date(Range.To.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountTo ?? CountToDefault).Year;
            }
        }

        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                foreach (var year in Enumerable.Range(YearFrom, YearTo - YearFrom + 1))
                {
                    var yearMonth = new YearMonth {Year = year, Month = Month};

                    yield return yearMonth.ToLocalDate(DayOfYear, RollStrategy);
                }
            }
        }
    }
}
