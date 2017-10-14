using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByOffset : ScheduleAbstracts.Repeating
    {
        [DataMember]
        public LocalDate InitialDate;
        [DataMember]
        public string Interval;

        public ByOffset(
            LocalDate initialDate,
            string interval,
            IRangeDate range = null,
            int? count = null)
        {
            InitialDate = initialDate;
            Interval = interval;

            if (range != null)
            {
                EdgeRange = new EdgeRangeDate(range);
            }
            CountTo = count;
        }

        public static ByOffset Create(
            LocalDate initialDate,
            string interval,
            IRangeDate range = null,
            int? count = null)
        {
            return new ByOffset(
                initialDate: initialDate,
                interval: interval,
                range: range,
                count: count);
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var results = new List<IDate>();

            EdgeRange?.Range.Validate();

            var iterDate = InitialDate;

            while (EdgeRange?.Range.Contains(iterDate) == true
                && results.Count <= (CountTo ?? CountToDefault))
            {
                results.Add(new Date(iterDate));

                iterDate = DateAdjuster.Adjust(iterDate, Interval);
            }

            results.Sort();

            return results;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByOffset>(db);
            base.Save(db, clock);
        }
    }
}
