using NodaTime;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class DateList : PersitableEntity, ISchedule
    {
        public DateList()
        {
        }

        public IEnumerable<LocalDate> Items
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Dates => Items.ToList();

        public void Save(IArangoDatabase db)
        {
            Save<DateList>(db);
        }
    }
}
