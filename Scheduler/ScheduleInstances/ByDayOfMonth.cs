using NodaTime;
using System.Collections.Generic;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfMonth : ScheduleAbstracts.RepeatingDay
    {
        public int DayOfMonth;
        public IClock Clock;

        public ByDayOfMonth()
        {
            CountFromDefault = 0;
            CountToDefault = 12;

            DayOfMonth = 1;
        }

        protected YearMonth YearMonthFrom
        {
            get
            {
                if (Range.From.HasValue)
                    return Range.From.Value.ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
            }
        }

        protected YearMonth YearMonthTo
        {
            get
            {
                if (Range.To.HasValue)
                    return Range.To.Value.ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountTo ?? CountToDefault);
            }
        }

        public override IEnumerable<LocalDate> Dates
        {
            get
            {
                var o = new List<LocalDate>();

                var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, Increment);

                foreach (var yearMonth in yearMonths)
                {
                    LocalDate localDate;

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