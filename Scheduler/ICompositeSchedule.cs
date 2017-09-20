using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler
{
    public interface ICompositeSchedule : ISchedule
    {
        IEdgeVertexs<ISchedule> InclusionsEdges { get; set; }
        IEdgeVertexs<ISchedule> ExclusionsEdges { get; set; }
        IEdgeVertexs<IDateRange> Breaks { get; set; }
    }
}
