using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : Schedule
    {
        public SingleDay()
        {
        }

        public Scheduler.Date Date
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates
        {
            get { yield return Date; }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save<SingleDay>(db);
        }
    }
}
