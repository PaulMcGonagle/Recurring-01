using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeRangeTime : IEdgeVertex<IRangeTime>
    {
        IRangeTime Range { get; set; }
    }
}