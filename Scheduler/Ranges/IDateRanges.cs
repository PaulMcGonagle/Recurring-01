using System.Collections.Generic;
using NodaTime;

namespace Scheduler.Ranges
{
    public interface IDateRanges : IList<DateRange>
    {
        bool Contains(Date date);
        bool Contains(LocalDate localDate);
    }
}