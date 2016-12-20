using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : PersistableEntity, ISchedule
    {
        public SingleDay()
        {
        }

        public Scheduler.Date Date
        {
            get;
            set;
        }

        public IEnumerable<Scheduler.Date> Dates
        {
            get { yield return Date; }
        }

        public SaveResult Save(IArangoDatabase db)
        {
            return Save<SingleDay>(db);
        }
    }
}
