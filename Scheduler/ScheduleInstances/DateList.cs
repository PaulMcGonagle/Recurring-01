using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<IDate> Items
        {
            get;
            set;
        }

        public override IEnumerable<IDate> Generate()
        {
            return Items
                .ToList();
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<DateList>(db);
            base.Save(db, clock);
        }
    }
}
