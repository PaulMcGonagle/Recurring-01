using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public interface IRangeDate : IVertex
    {
        EdgeDate Start { get; }
        EdgeDate End { get; }

        bool Contains(LocalDate localDate);

        void Validate();
    }
}