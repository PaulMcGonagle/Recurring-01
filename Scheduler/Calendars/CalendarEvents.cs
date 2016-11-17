using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Calendars
{
    public class CalendarEvents : List<CalendarEvent>
    {
        public IEnumerable<Appointment> Occurrences()
        {
            return this.SelectMany(ce => ce.Occurrences());
        }
    }
}
