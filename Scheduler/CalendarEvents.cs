using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class CalendarEvents : List<Scheduler.CalendarEvent>
    {
        public IEnumerable<Appointment> Occurrences()
        {
            return this.SelectMany(ce => ce.Occurrences());
        }
    }
}
