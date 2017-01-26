using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeRangeTime : IEdgeVertex<ITimeRange>
    {
        ITimeRange Range { get; set; }
    }
}