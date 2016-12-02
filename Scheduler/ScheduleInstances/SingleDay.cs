using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : PersitableEntity, ISchedule
    {
        public SingleDay()
        {
        }

        public LocalDate Date
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Dates
        {
            get { yield return Date; }
        }

        public void Save(IArangoDatabase db)
        {
            Save<SingleDay>(db);
        }
    }
}
