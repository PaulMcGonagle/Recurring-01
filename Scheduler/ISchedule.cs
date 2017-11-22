using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISchedule : IVertex
    {
        IEnumerable<IDate> Generate(IClock clock);

        IScheduleInstance ScheduleInstance { get; }
    }
}
