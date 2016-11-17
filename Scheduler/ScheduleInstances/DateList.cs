using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.ScheduleInstances
{
    public class DateList : ISchedule
    {
        public DateList()
        {
        }

        public IEnumerable<LocalDate> Items
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Dates()
        {
            return Items.ToList();
        }
    }
}
