using System;
using System.Collections.Generic;
using NodaTime;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Generation;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : Schedule
    {
        [IgnoreDataMember]
        public IEdgeRangeDate EdgeRange;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }

        protected int Increment = 1;

        public abstract override IEnumerable<IDate> Generate(IClock clock);
        
        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            base.Save(db, clock);
            EdgeRange?.Save(db, clock, this);
        }

        #endregion
    }
}
