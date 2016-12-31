using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;

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

        [IgnoreDataMember]
        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }

            set
            {
                _clock = value;
            }
        }

        [IgnoreDataMember]
        protected YearMonth YearMonthFrom
        {
            get
            {
                if (Range?.From != null)
                    return new Date(Range.From.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
            }
        }

        [IgnoreDataMember]
        protected YearMonth YearMonthTo
        {
            get
            {
                if (Range?.To != null)
                    return new Date(Range.To.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountTo ?? CountToDefault);
            }
        }

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates
        {
            get
            {
                var o = new List<Date>();

                var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, Increment);

                foreach (var yearMonth in yearMonths)
                {
                    Date localDate;

                    if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                    {
                        o.Add(localDate);
                    }
                }

                return o;
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save<ByDayOfMonth>(db);
        }
    }
}