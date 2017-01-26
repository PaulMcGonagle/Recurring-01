using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public interface IDateRange : IVertex
    {
        EdgeDate From { get; }
        EdgeDate To { get; }

        bool Contains(LocalDate localDate);

        void Validate();
    }
}