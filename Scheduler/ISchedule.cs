using System.Collections.Generic;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISchedule : IVertex
    {
        IEnumerable<IDate> Generate(IClock clock);
    }
}
