using System;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Days;

        [IgnoreDataMember]
        public IClock Clock { get; set; }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates
        {
            get
            {
                var start = EdgeRange.ToVertex.From ?? DateTimeHelper.GetToday(Clock).AddWeeks(-(CountFrom ?? CountFromDefault));
                var end = EdgeRange.ToVertex.To ?? DateTimeHelper.GetToday(Clock).AddWeeks((CountTo ?? CountToDefault));

                var range = DateTimeHelper.Range(start: start, end: end);
                return range.Where(d => Days.Contains(d.IsoDayOfWeek));
            }
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<ByWeekdays>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
