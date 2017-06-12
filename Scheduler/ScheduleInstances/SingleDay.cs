using System;
using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : Schedule
    {
        public Date Date
        {
            get;
            set;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            yield return Date;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<SingleDay>(db);
            base.Save(db, clock);
        }
    }
}
