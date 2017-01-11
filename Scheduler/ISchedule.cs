using System.Collections.Generic;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISchedule : IVertex
    {
        IEnumerable<GeneratedDate> GenerateDates();
    }
}
