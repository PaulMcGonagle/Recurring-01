using NodaTime;
using System.Collections.Generic;
using ArangoDB.Client;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class ByRangeDate : ScheduleAbstracts.Repeating
    {
        public ByRangeDate()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var start = EdgeRangeDate.ToVertex.Start.Date ?? DateTimeHelper.GetToday(clock).PlusDays(-(CountFrom ?? CountFromDefault));
            var end = EdgeRangeDate.ToVertex.End.Date ?? DateTimeHelper.GetToday(clock).PlusDays((CountTo ?? CountToDefault));

            return DateTimeHelper.Range(start: start, end: end);
        }

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            EdgeRangeDate.Save(db, clock, schedule, "HasRange");
        }
    }

    public class ByRangeDateBuilder : RepeatingBuilder
    {
        private readonly ByRangeDate _byRangeDate;

        protected override Repeating Repeating => _byRangeDate;

        public ByRangeDateBuilder()
        {
            _byRangeDate = new ByRangeDate();
        }

        public ByRangeDate Build()
        {
            _byRangeDate.Validate();

            return _byRangeDate;
        }
    }
}
