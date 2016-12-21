using System;
using System.Globalization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Range : Vertex
    {
        public Scheduler.Date From { get; }
        public Scheduler.Date To { get; }

        public Range(int fromYear, YearMonth.MonthValue fromMonth, int fromDay, int toYear, YearMonth.MonthValue toMonth,
            int toDay)
        {
            From = new Scheduler.Date(fromYear, fromMonth, fromDay);
            To = new Scheduler.Date(toYear, toMonth, toDay);
        }

        public Range(Scheduler.Date from, Scheduler.Date to)
        {
            if (from.Value > to.Value)
                throw new ArgumentOutOfRangeException(nameof(from), $"From date [{to.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{from.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            From = from;
            To = to;
        }

        public Range Save(IArangoDatabase db)
        {
            Save<Range>(db);

            return this;
        }
    }
}
