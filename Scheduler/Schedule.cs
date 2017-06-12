using System;
using System.Collections.Generic;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        public abstract IEnumerable<IDate> Generate(IClock clock);
    }
}
