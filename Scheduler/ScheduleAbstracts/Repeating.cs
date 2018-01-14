using System.Collections.Generic;
using System.Linq;
using NodaTime;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : ScheduleInstance
    {
        [IgnoreDataMember] public IEdgeRangeDate EdgeRangeDate;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }

        protected int Increment = 1;

        public override void Validate()
        {
        }

        public IEdgeVertexs<ISchedule> Exclusions { get; set; }

        public virtual bool Contains(IClock clock, IDate date)
        {
            var dates = Generate(clock);

            return dates.Contains(date);
        }

        public abstract override IEnumerable<IDate> Generate(IClock clock);

        #region Save

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            EdgeRangeDate?.Save(db, clock, schedule, "HasRangeDate");

            foreach (var exclusion in Exclusions)
            {
                exclusion.Save(db, clock, schedule, "HasExclusion");
            }
        }

        #endregion Save

        public abstract class Builder
        {
            protected abstract Repeating Repeating { get; }

            public EdgeRangeDate EdgeRangeDate
            {
                set => Repeating.EdgeRangeDate = value;
            }

            public IRangeDate RangeDate
            {
                set => EdgeRangeDate = new EdgeRangeDate(value);
            }

            public IEdgeVertexs<ISchedule> Exclusions
            {
                set => Repeating.Exclusions = value;
            }

            public int? CountFrom
            {
                set => Repeating.CountFrom = value;
            }

            public int? CountTo
            {
                set => Repeating.CountTo = value;
            }

            public int CountFromDefault
            {
                set => Repeating.CountFromDefault = value;
            }

            public int CountToDefault
            {
                set => Repeating.CountToDefault = value;
            }
        }
    }
}