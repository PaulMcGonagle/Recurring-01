using NodaTime;
using System.Collections.Generic;
using ArangoDB.Client;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class ByRangeDate : Repeating
    {
        public ByRangeDate()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var start = EdgeRangeDate.ToVertex.Start.Date ??
                        DateTimeHelper.GetToday(clock).PlusDays(-(CountFrom ?? CountFromDefault));
            var end = EdgeRangeDate.ToVertex.End.Date ?? DateTimeHelper.GetToday(clock)
                          .PlusDays((CountTo ?? CountToDefault));

            return DateTimeHelper.Range(start: start, end: end);
        }

        public override bool Contains(IClock clock, IDate date)
        {
            return EdgeRangeDate.RangeDate.Contains(date.Value);
        }

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            EdgeRangeDate.Save(db, clock, schedule, "HasRange");
        }

        public new class Builder : Repeating.Builder
        {
            private readonly ByRangeDate _byRangeDate;

            protected override Repeating Repeating => _byRangeDate;

            public Builder()
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
}