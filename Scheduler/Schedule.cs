using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        //[IgnoreDataMember]
        public abstract IEnumerable<Date> GenerateDates();
    }
}
