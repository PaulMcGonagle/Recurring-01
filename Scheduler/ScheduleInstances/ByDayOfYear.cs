using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Ranges;
using Scheduler.ScheduleAbstracts;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfYear : ScheduleAbstracts.RepeatingDay
    {
        private int _dayOfYear = 1;
        private IClock _clock;

        public int DayOfYear
        {
            get => _dayOfYear;
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
            set { _clock = value; }
        }

        [IgnoreDataMember]
        public YearMonth.MonthValue Month { get; set; } = YearMonth.MonthValue.January;

        [IgnoreDataMember]
        protected int YearFrom
        {
            get
            {
                if (EdgeRangeDate?.ToVertex?.Start != null)
                    return new Date(EdgeRangeDate.ToVertex.Start.Date.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountFrom ?? CountFromDefault).Year;
            }
        }

        [IgnoreDataMember]
        protected int YearTo
        {
            get
            {
                if (EdgeRangeDate?.ToVertex?.End != null)
                    return new Date(EdgeRangeDate.ToVertex.End.Date.Value).ToYearMonth().Year;

                var thisMonth = Clock.GetLocalYearMonth();

                return thisMonth.AddYears(CountTo ?? CountToDefault).Year;
            }
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            if (_clock == null)
                _clock = clock;

            var generatedDates = new List<IDate>();

            generatedDates
                .AddRange(Enumerable.Range(YearFrom, YearTo - YearFrom + 1)
                    .Select(year => (new YearMonth {Year = year, Month = Month}).ToLocalDate(DayOfYear, RollStrategy)));

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

        public IRangeDate Range
        {
            set => _byDayOfYear.EdgeRangeDate = new EdgeRangeDate(value);
        }

        public int CountTo
        {
            set => _byDayOfYear.CountTo = value;
        }

        public int CountFrom
        {
            set => _byDayOfYear.CountFrom = value;
        }

        public ByDayOfYear Build()
        {
            _byDayOfYear.Validate();

            return _byDayOfYear;
        }
    }
}
