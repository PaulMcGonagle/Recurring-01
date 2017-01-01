using System.Collections.Generic;
using System.Runtime.Serialization;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        [IgnoreDataMember]
        public virtual IEnumerable<Date> Dates { get; }
    }
}
