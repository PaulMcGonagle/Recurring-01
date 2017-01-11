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

        public override IEnumerable<GeneratedDate> GenerateDates()
        {
            return Items.Select(i => new GeneratedDate(
                source: this,
                date: i))
                .ToList();
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
