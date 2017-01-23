using NodaTime;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public interface IDateRange
    {
        EdgeDate From { get; }
        EdgeDate To { get; }

        bool Contains(LocalDate localDate);

        void Validate();
    }
}