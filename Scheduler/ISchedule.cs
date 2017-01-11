using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISchedule : IVertex
    {
        GeneratedDates Generate();
    }
}
