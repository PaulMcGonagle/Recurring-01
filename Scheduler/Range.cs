using System;
using System.Globalization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Range : Vertex
    {
        public Date From { get; }
        public Date To { get; }

        public Range(int fromYear, YearMonth.MonthValue fromMonth, int fromDay, int toYear, YearMonth.MonthValue toMonth,
            int toDay)
        {
            From = new Date(fromYear, fromMonth, fromDay);
            To = new Date(toYear, toMonth, toDay);
        }

        public Range(Date from, Date to)
        {
            if (from.Value > to.Value)
                throw new ArgumentOutOfRangeException(nameof(from), $"From date [{to.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{from.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            From = from;
            To = to;
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Range>(db),
                () => base.Save(db),
            });
        }
    }
}
