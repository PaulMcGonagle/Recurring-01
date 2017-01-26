using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeSchedule : IEdgeVertex<ISchedule>
    {
        ISchedule Schedule { get; }
    }
}