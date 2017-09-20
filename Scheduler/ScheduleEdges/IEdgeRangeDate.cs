using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public interface IEdgeRangeDate : IEdgeVertex<IRangeDate>
    {
        IRangeDate Range { get; set; }
    }
}