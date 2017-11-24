using System.Collections.Generic;
using NodaTime;
using System.Runtime.Serialization;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfMonth : RepeatingDay
    {
        [DataMember] public int DayOfMonth;

        public ByDayOfMonth()
        {
            CountFromDefault = 0;
            CountToDefault = 12;

            DayOfMonth = 1;
        }

        protected YearMonth GetYearMonthFrom(IClock clock)
        {
            if (EdgeRangeDate?.ToVertex?.Start != null)
                return new Date(EdgeRangeDate.ToVertex.Start.Date.Value).ToYearMonth();

            var yearMonth = clock.GetLocalYearMonth();

            return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
        }

        protected YearMonth GetYearMonthTo(IClock clock)
        {
            if (EdgeRangeDate?.ToVertex?.End != null)
                return new Date(EdgeRangeDate.ToVertex.End.Date.Value).ToYearMonth();

            var yearMonth = clock.GetLocalYearMonth();

            return yearMonth.AddMonths(CountTo ?? CountToDefault);
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var yearMonthFrom = GetYearMonthFrom(clock);
            var yearMonthTo = GetYearMonthTo(clock);

            var yearMonths = YearMonth.Range(yearMonthFrom, yearMonthTo, Increment);

            foreach (var yearMonth in yearMonths)
            {

                if (yearMonth.TryToLocalDate(DayOfMonth, out Date localDate, RollStrategy))
                {
                    yield return localDate;
                }
            }
        }

        public new class Builder : RepeatingDay.Builder
        {
            private readonly ByDayOfMonth _byDayOfMonth;

            protected override RepeatingDay RepeatingDay => _byDayOfMonth;

            public Builder()
            {
                _byDayOfMonth = new ByDayOfMonth
                {
                    CountFromDefault = 0,
                    CountToDefault = 12,
                    DayOfMonth = 1
                };
            }


            public int DayOfMonth
            {
                set => _byDayOfMonth.DayOfMonth = value;
            }

            public ByDayOfMonth Build()
            {
                _byDayOfMonth.Validate();

                return _byDayOfMonth;
            }
        }
    }
}