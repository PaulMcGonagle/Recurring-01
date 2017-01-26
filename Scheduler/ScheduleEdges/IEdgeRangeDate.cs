using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeRangeDate : IEdgeVertex<IDateRange>
    {
        IDateRange Range { get; set; }
    }
}