using System;
using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : Schedule
    {
        public Date Date
        {
            get;
            set;
        }

        public override IEnumerable<Date> GenerateDates()
        {
            yield return Date;
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<SingleDay>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
