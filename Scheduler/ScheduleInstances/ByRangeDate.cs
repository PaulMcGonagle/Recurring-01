using NodaTime;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByRangeDate : ScheduleAbstracts.Repeating
    {
        public ByRangeDate()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public static ByRangeDate Create(
            IRangeDate rangeDate)
        {
            return new ByRangeDate()
            {
                EdgeRange = new EdgeRangeDate(rangeDate),
            };
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var start = EdgeRange.ToVertex.From.Date ?? DateTimeHelper.GetToday(clock).PlusDays(-(CountFrom ?? CountFromDefault));
            var end = EdgeRange.ToVertex.To.Date ?? DateTimeHelper.GetToday(clock).PlusDays((CountTo ?? CountToDefault));

            return DateTimeHelper.Range(start: start, end: end);
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByRangeDate>(db);
            EdgeRange.Save(db, clock, this, "HasRange");
            base.Save(db, clock);
        }
    }
}
