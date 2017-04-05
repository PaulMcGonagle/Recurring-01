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
        private IClock _clock;

        [IgnoreDataMember]
        public IClock Clock
        {
            get { return _clock ?? (_clock = SystemClock.Instance); }
            set { _clock = value; }
        }

        public ByWeekdays(
            IEnumerable<IsoDayOfWeek> weekdays,
            IClock clock)
        {
            Days = weekdays;
            Clock = clock;
        }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public static ByWeekdays Create(
            IEnumerable<IsoDayOfWeek> weekdays,
            DateRange dateRange,
            IClock clock)
        {
            return new ByWeekdays(
                weekdays: weekdays,
                clock: clock)
            {
                EdgeRange = new EdgeRangeDate(dateRange),
            };
        }

        public override IEnumerable<IDate> Generate()
        {
            var start = EdgeRange.ToVertex.From.Date ?? DateTimeHelper.GetToday(Clock).AddWeeks(-(CountFrom ?? CountFromDefault));
            var end = EdgeRange.ToVertex.To.Date ?? DateTimeHelper.GetToday(Clock).AddWeeks((CountTo ?? CountToDefault));

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
