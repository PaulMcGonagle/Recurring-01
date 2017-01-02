using System.Collections.Generic;
using System.Runtime.Serialization;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        //[IgnoreDataMember]
        public abstract IEnumerable<Date> GenerateDates();
    }
}
