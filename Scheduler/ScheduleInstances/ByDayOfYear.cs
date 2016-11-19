using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfYear : ScheduleAbstracts.RepeatingDay
    {
        private int dayOfYear = 1;
        public IClock Clock;
        private YearMonth.MonthValue month = YearMonth.MonthValue.January;

        public int DayOfYear
        {
            get { return dayOfYear; }
            set
            {
                if (value < 1 || value > 31)
                {
                    throw new ArgumentOutOfRangeException($"Invalid DayOfYear value '{value}'");
                }

                dayOfYear = value;
            }
        }

        public YearMonth.MonthValue Month
        {
            get { return month; }
            set
            { 
                month = value;
            }
        }

        protected int YearFrom
        {
            get
            {
                if (DateFrom.HasValue)
                    return DateFrom.Value.ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountFrom ?? CountFromDefault).Year;
            }
        }

        protected int YearTo
        {
            get
            {
                if (DateTo.HasValue)
                    return DateTo.Value.ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountTo ?? CountToDefault).Year;
            }
        }

        public override IEnumerable<LocalDate> Dates()
        {
            foreach (var year in Enumerable.Range(YearFrom, YearTo - YearFrom + 1))
            {
                var yearMonth = new YearMonth { Year = year, Month = this.Month };

                yield return yearMonth.ToLocalDate(DayOfYear, RollStrategy);
            }
        }
    }
}
