using System.Collections.Generic;

namespace Scheduler
{
    public interface ICalendarEvent
    {
        IEnumerable<Appointment> Occurrences();
    }
}
