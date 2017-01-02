using System.Collections.Generic;

namespace Scheduler
{
    public interface ISchedule
    {
        IEnumerable<Date> GenerateDates();
    }
}
