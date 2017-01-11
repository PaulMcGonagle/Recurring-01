using System;
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
        public EdgeRange EdgeRange;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }

        protected int Increment = 1;

        public override GeneratedDates Generate()
        {
            throw new NotImplementedException();
        }

#region Save

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            if (EdgeRange == null)
                return SaveResult.Incomplete;

            var results = base.Save(db, clock);

            if (results != SaveResult.Success) return results;

            return EdgeRange.Save(db, clock, this);
        }

#endregion
    }
}
