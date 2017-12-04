using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public interface IRangeDate : IVertex
    {
        IEdgeDate Start { get; }
        IEdgeDate End { get; }

        bool Contains(LocalDate localDate);
    }
}