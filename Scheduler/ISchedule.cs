using NodaTime;
using System.Collections.Generic;

namespace Scheduler
{
    public interface ISchedule
    {
        IEnumerable<LocalDate> Occurrences();
    }
}
