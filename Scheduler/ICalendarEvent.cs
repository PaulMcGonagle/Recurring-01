using System.Collections.Generic;

namespace Scheduler
{
    public interface ICalendarEvent
    {
        IEnumerable<Episode> Episodes();
    }
}
