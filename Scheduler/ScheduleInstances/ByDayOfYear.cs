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
                if (EdgeRange?.ToVertex?.From != null)
                    return new Date(EdgeRange.ToVertex.From.Date.Value).ToYearMonth().Year;

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
                    return new Date(EdgeRange.ToVertex.To.Date.Value).ToYearMonth().Year;

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

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByDayOfYear>(db);
            base.Save(db, clock);
        }
    }
}
