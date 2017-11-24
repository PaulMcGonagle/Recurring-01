using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CoreLibrary;
using Scheduler.Ranges;
using Scheduler.ScheduleAbstracts;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfYear : ScheduleAbstracts.RepeatingDay
    {
        public int? DayOfYear { get; internal set; }

        public YearMonth.MonthValue? Month { get; internal set; }

        public int? YearFrom { get; internal set; }
        public int? YearTo { get; internal set; }

        protected int GetYearFrom(IClock clock)
        {
            if (EdgeRangeDate?.ToVertex?.Start != null)
                return new Date(EdgeRangeDate.ToVertex.Start.Date.Value).ToYearMonth().Year;

            var thisMonth = clock.GetLocalYearMonth();

            return thisMonth.AddYears(CountFrom ?? CountFromDefault).Year;
        }

        protected int GetYearTo(IClock clock)
        {
            if (EdgeRangeDate?.ToVertex?.End != null)
                return new Date(EdgeRangeDate.ToVertex.End.Date.Value).ToYearMonth().Year;

            var thisMonth = clock.GetLocalYearMonth();

            return thisMonth.AddYears(CountTo ?? CountToDefault).Year;
        }

        public override void Validate()
        {
            //do not validate base. EdgeRange may be null
            //base.Validate();

            if (!DayOfYear.HasValue)
                throw new ArgumentOutOfRangeException(nameof(DayOfYear), "Must have value");

            if (DayOfYear.Value < 1)
                throw new ArgumentOutOfRangeException(nameof(DayOfYear), "Must be > 1");

            if (DayOfYear.Value > 31)
                throw new ArgumentOutOfRangeException(nameof(DayOfYear), "Must be <= 31");

            if (!Month.HasValue)
                throw new ArgumentOutOfRangeException(nameof(Month), "Must have value");
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            Validate();

            var generatedDates = new List<IDate>();

            var yearFrom = GetYearFrom(clock);
            var yearTo = GetYearTo(clock);

            generatedDates
                .AddRange(Enumerable.Range(yearFrom, yearTo - yearFrom + 1)
                    .Select(year => (new YearMonth {Year = year, Month = Month.Value}).ToLocalDate(DayOfYear.Value, RollStrategy)));

            return generatedDates;
        }
    }

    public class ByDayOfYearBuilder : RepeatingDayBuilder
    {
        private readonly ByDayOfYear _byDayOfYear;

        protected override RepeatingDay RepeatingDay => _byDayOfYear;

        public ByDayOfYearBuilder()
        {
            _byDayOfYear = new ByDayOfYear();
        }

        public int? DayOfYear
        {
            set => _byDayOfYear.DayOfYear = value;
        }

        public YearMonth.MonthValue? Month
        {
            set => _byDayOfYear.Month = value;
        }

        public int? YearFrom
        {
            set => _byDayOfYear.YearFrom = value;
        }

        public int? YearTo
        {
            set => _byDayOfYear.YearTo = value;
        }

        public ByDayOfYear Build()
        {
            _byDayOfYear.Validate();

            return _byDayOfYear;
        }
    }
}
