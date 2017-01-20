using NodaTime;

namespace Scheduler.Ranges
{
    public interface IDateRange
    {
        Date From { get; }
        Date To { get; }

        bool Contains(LocalDate localDate);

        void Validate();
    }
}