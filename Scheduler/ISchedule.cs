using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISchedule : IVertex
    {
        IEnumerable<Date> GenerateDates();
    }
}
