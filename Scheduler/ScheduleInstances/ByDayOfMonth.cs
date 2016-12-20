using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfMonth : ScheduleAbstracts.RepeatingDay
    {
        public int DayOfMonth;
        private IClock _clock;

        public ByDayOfMonth()
        {
            CountFromDefault = 0;
            CountToDefault = 12;

            DayOfMonth = 1;
        }

        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        protected YearMonth YearMonthFrom
        {
            get
            {
                if (Range?.From != null)
                    return new Scheduler.Date(Range.From.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
            }
        }

        protected YearMonth YearMonthTo
        {
            get
            {
                if (Range?.To != null)
                    return new Scheduler.Date(Range.To.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountTo ?? CountToDefault);
            }
        }

        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                var o = new List<Scheduler.Date>();

                var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, Increment);

                foreach (var yearMonth in yearMonths)
                {
                    Scheduler.Date localDate;

                    if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                    {
                        o.Add(localDate);
                    }
                }

                return o;
            }
        }
    }
}