﻿using System;
using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Generation;

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

        public override GeneratedDates Generate()
        {
            var dates = new GeneratedDates();

            var yearMonths = YearMonth.Range(YearMonthFrom, YearMonthTo, Increment);

            foreach (var yearMonth in yearMonths)
            {
                Date localDate;

                if (yearMonth.TryToLocalDate(DayOfMonth, out localDate, RollStrategy))
                {
                    dates.Add(new GeneratedDate(
                        source: this,
                        date: localDate));
                }
            }

            return dates;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByDayOfMonth>(db);
            base.Save(db, clock);
        }
    }
}