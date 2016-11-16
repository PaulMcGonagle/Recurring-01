using NodaTime;
using System.Collections.Generic;

namespace Scheduler
{
    public abstract class ScheduleBase
    {
        public abstract IEnumerable<LocalDate> Occurrences();
    }
}
