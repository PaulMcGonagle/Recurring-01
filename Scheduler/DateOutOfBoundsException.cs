using System;

namespace Scheduler
{
    public class DateOutOfBoundsException : Exception
    {
        public DateOutOfBoundsException(int year, YearMonth.MonthValue month, int day)
            : base(string.Format("Unable to determine date {0}-{1}-{2}", year, month, day))
        {
        }
    }
}
