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
                if (DateFrom.HasValue)
                    return DateFrom.Value.ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
            }
        }

        protected YearMonth YearMonthTo
        {
            get
            {
                if (DateTo.HasValue)
                    return DateTo.Value.ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountTo ?? CountToDefault);
            }
        }

        public override IEnumerable<LocalDate> Dates()
        {
            var o = new List<LocalDate>();

            var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, increment);

            foreach (var yearMonth in yearMonths)
            {
                LocalDate? localDate;

                if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                {
                    o.Add(localDate.Value);
                }
            }

            return o;
        }
    }
}