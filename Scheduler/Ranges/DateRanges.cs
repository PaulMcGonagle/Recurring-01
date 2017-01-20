using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Scheduler.Ranges
{
    public class DateRanges : List<DateRange>, IDateRanges
    {
        public bool Contains(Date date)
        {
            return Contains(date.Value);
        }

        public bool Contains(LocalDate localDate)
        {
            return this.Exists(dr => dr.Contains(localDate));
        }
    }
}
