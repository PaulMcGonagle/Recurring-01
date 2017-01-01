using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : Schedule
    {
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
            var results = Save<SingleDay>(db);

            if (results != SaveResult.Success) return results;

            return base.Save(db);
        }
    }
}
