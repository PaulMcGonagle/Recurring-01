using System;
using System.Collections.Generic;
using NodaTime;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : ScheduleInstance
    {
        [IgnoreDataMember]
        public IEdgeRangeDate EdgeRangeDate;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }

        protected int Increment = 1;

        public override void Validate()
        {
            Guard.AgainstNull(EdgeRangeDate, nameof(EdgeRangeDate));
        }

        public abstract override IEnumerable<IDate> Generate(IClock clock);
        
        #region Save

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            EdgeRangeDate?.Save(db, clock, schedule);
        }

        #endregion
    }

    public abstract class RepeatingBuilder
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

        public int? CountFrom { set => Repeating.CountFrom = value; }
        public int? CountTo { set => Repeating.CountTo = value; }
        public int CountFromDefault { set => Repeating.CountFromDefault = value; }
        public int CountToDefault { set => Repeating.CountToDefault = value; }
    }
}
