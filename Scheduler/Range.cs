using System;
using System.Globalization;
using NodaTime;

namespace Scheduler
{
    public struct Range
    {
        public LocalDate? From { get; }
        public LocalDate? To { get; }

        public Range(int fromYear, YearMonth.MonthValue fromMonth, int fromDay, int toYear, YearMonth.MonthValue toMonth,
            int toDay)
        {
            From = DateTimeHelper.GetLocalDate(fromYear, fromMonth, fromDay);
            To = DateTimeHelper.GetLocalDate(toYear, toMonth, toDay);
        }

        public Range(LocalDate from, LocalDate to)
        {
            if (from > to)
                throw new ArgumentOutOfRangeException(nameof(from), $"From date [{to.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{from.ToString("D", CultureInfo.CurrentCulture)}]");

            From = from;
            To = to;
        }
    }
}
