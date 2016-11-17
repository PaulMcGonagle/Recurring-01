using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Calendars
{
    public class CalendarEvents : List<CalendarEvent>
    {
        public IEnumerable<Episode> Episodes()
        {
            return this.SelectMany(ce => ce.Episodes());
        }
    }
}
