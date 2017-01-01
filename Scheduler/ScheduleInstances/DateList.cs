using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<Scheduler.Date> Items
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates => Items.ToList();

        public override SaveResult Save(IArangoDatabase db)
        {
            var results = Save<DateList>(db);

            if (results != SaveResult.Success) return results;

            return base.Save(db);
        }
    }
}
