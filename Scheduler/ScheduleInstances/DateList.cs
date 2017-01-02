using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<Date> Items
        {
            get;
            set;
        }

        public override IEnumerable<Scheduler.Date> GenerateDates()
        {
            return Items.ToList();
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<DateList>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
