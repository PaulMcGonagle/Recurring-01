using System;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.Users;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Days;

        public ByWeekdays(
            IEnumerable<IsoDayOfWeek> weekdays)
        {
            Days = weekdays;
        }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public static ByWeekdays Create(
            IEnumerable<IsoDayOfWeek> weekdays,
            IDateRange dateRange)
        {
            return new ByWeekdays(
                weekdays: weekdays)
            {
                EdgeRange = new EdgeRangeDate(dateRange),
            };
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var start = EdgeRange.ToVertex.From.Date ?? DateTimeHelper.GetToday(clock).AddWeeks(-(CountFrom ?? CountFromDefault));
            var end = EdgeRange.ToVertex.To.Date ?? DateTimeHelper.GetToday(clock).AddWeeks((CountTo ?? CountToDefault));

            var range = DateTimeHelper.Range(start: start, end: end);


            return range.Where(d => Days.Contains(d.IsoDayOfWeek));
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByWeekdays>(db);
            base.Save(db, clock);
        }
    }
}
