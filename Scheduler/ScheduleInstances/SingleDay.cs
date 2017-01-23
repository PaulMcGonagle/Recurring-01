using System;
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

        public override GeneratedDates Generate()
        {
            var results = new GeneratedDates
            {
                new GeneratedDate(
                    source: this,
                    date: Date)
            };


            return results;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<SingleDay>(db);
            base.Save(db, clock);
        }
    }
}
