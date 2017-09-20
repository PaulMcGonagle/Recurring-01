using System.Collections.Generic;
using NodaTime;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class ByDayOfMonth : ScheduleAbstracts.RepeatingDay
    {
        [DataMember]
        public int DayOfMonth;

        private IClock _clock;

        public ByDayOfMonth()
        {
            CountFromDefault = 0;
            CountToDefault = 12;

            DayOfMonth = 1;
        }

        [IgnoreDataMember]
        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }
            set { _clock = value; }
        }

        protected YearMonth GetYearMonthFrom(IClock clock)
        {
            if (EdgeRange?.ToVertex?.From != null)
                return new Date(EdgeRange.ToVertex.From.Date.Value).ToYearMonth();

            var yearMonth = Clock.GetLocalYearMonth();

            return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
        }

        protected YearMonth GetYearMonthTo(IClock clock)
        {
            if (EdgeRange?.ToVertex?.To != null)
                return new Date(EdgeRange.ToVertex.To.Date.Value).ToYearMonth();

            var yearMonth = Clock.GetLocalYearMonth();

            return yearMonth.AddMonths(CountTo ?? CountToDefault);
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var yearMonthFrom = GetYearMonthFrom(clock);
            var yearMonthTo = GetYearMonthTo(clock);

            var yearMonths = YearMonth.Range(yearMonthFrom, yearMonthTo, Increment);

            foreach (var yearMonth in yearMonths)
            {
                Date localDate;

                if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                {
                    yield return localDate;
                }
            }
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByDayOfMonth>(db);
            base.Save(db, clock);
        }
    }
}