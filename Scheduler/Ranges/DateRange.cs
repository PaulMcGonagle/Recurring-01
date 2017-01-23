﻿using System;
using System.Globalization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public class DateRange : Vertex, IDateRange
    {
        public Date From { get; }
        public Date To { get; }

        public DateRange(int fromYear, YearMonth.MonthValue fromMonth, int fromDay, int toYear, YearMonth.MonthValue toMonth,
            int toDay)
        {
            From = new Date(fromYear, fromMonth, fromDay);
            To = new Date(toYear, toMonth, toDay);
        }

        public DateRange(Date from, Date to)
        {
            if (from.Value > to.Value)
                throw new ArgumentOutOfRangeException(nameof(from), $"From date [{to.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{from.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            From = from;
            To = to;
        }

        public void Validate()
        {
            if (From?.Value != null && To?.Value == null && From.Value <= To.Value)
            {
                throw new ArgumentOutOfRangeException($"Range is invalid: From={From?.Value}, To={To?.Value}");
            }
        }

        public bool Contains(LocalDate localDate)
        {
            return From.Value <= localDate && localDate <= To.Value;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<DateRange>(db);
            base.Save(db, clock);
        }
    }
}
