using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        public abstract GeneratedDates Generate();
    }
}
