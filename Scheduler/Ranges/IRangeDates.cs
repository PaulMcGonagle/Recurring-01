using System.Collections.Generic;
using NodaTime;

namespace Scheduler.Ranges
{
    public interface IRangeDates : IList<RangeDate>
    {
        bool Contains(Date date);
        bool Contains(LocalDate localDate);
    }
}