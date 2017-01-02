using System;
using NodaTime;
using System.Collections.Generic;
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
                if (EdgeRange?.ToVertex?.From != null)
                    return new Date(EdgeRange.ToVertex.From.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountFrom ?? CountFromDefault);
            }
        }

        [IgnoreDataMember]
        protected YearMonth YearMonthTo
        {
            get
            {
                if (EdgeRange?.ToVertex?.To != null)
                    return new Date(EdgeRange.ToVertex.To.Value).ToYearMonth();

                var yearMonth = Clock.GetLocalYearMonth();

                return yearMonth.AddMonths(CountTo ?? CountToDefault);
            }
        }

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates
        {
            get
            {
                var dates = new List<Date>();

                var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, Increment);

                foreach (var yearMonth in yearMonths)
                {
                    Date localDate;

                    if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                    {
                        dates.Add(localDate);
                    }
                }

                return dates;
            }
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<ByDayOfMonth>(db),
                () => base.Save(db, clock),
            });
        }
    }
}