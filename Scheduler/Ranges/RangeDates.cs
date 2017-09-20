using System.Collections.Generic;
using NodaTime;

namespace Scheduler.Ranges
{
    public class RangeDates : List<RangeDate>, IRangeDates
    {
        public bool Contains(Date date)
        {
            return Contains(date.Value);
        }

        public bool Contains(LocalDate localDate)
        {
            return Exists(dr => dr.Contains(localDate));
        }
    }
}
