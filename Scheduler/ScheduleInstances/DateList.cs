using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<Date> Items
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates => Items.ToList();

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<DateList>(db),
                () => base.Save(db),
            });
        }
    }
}
