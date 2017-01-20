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

        [IgnoreDataMember]
        public IClock Clock { get; set; }

        public ByWeekdays(
            IClock clock,
            IEnumerable<IsoDayOfWeek> weekdays)
        {
            Clock = clock;
            Days = weekdays;
        }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public static ByWeekdays Create(
            IClock clock,
            IEnumerable<IsoDayOfWeek> weekdays,
            DateRange dateRange)
        {
            return new ByWeekdays(
                clock: clock,
                weekdays: weekdays)
            {
                EdgeRange = new EdgeRange(dateRange),
            };
        }

        public override GeneratedDates Generate()
        {
            var start = EdgeRange.ToVertex.From ?? DateTimeHelper.GetToday(Clock).AddWeeks(-(CountFrom ?? CountFromDefault));
            var end = EdgeRange.ToVertex.To ?? DateTimeHelper.GetToday(Clock).AddWeeks((CountTo ?? CountToDefault));

            var range = DateTimeHelper.Range(start: start, end: end);

            var generatedDates = new GeneratedDates();

            generatedDates.AddRange(range.Where(d => Days.Contains(d.IsoDayOfWeek)).Select(d => new GeneratedDate(source: this, date: d)));

            return generatedDates;
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
