using NodaTime;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Vertex, ISchedule
    {
        public DateList()
        {
        }

        public IEnumerable<Scheduler.Date> Items
        {
            get;
            set;
        }

        public IEnumerable<Scheduler.Date> Dates => Items.ToList();

        public SaveResult Save(IArangoDatabase db)
        {
            return Save<DateList>(db);
        }
    }
}
