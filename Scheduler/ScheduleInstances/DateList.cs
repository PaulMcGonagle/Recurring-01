using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<Date> Items
        {
            get;
            set;
        }

        public override GeneratedDates Generate()
        {
            var generatedDates = new GeneratedDates();

            generatedDates.AddRange(
                Items.Select(i => new GeneratedDate(
                    source: this,
                    date: i))
                .ToList());

            return generatedDates;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<DateList>(db);
            base.Save(db, clock);
        }
    }
}
