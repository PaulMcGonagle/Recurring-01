using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IDate : IVertex
    {
        LocalDate Value { get; set; }

        IsoDayOfWeek IsoDayOfWeek { get; }

        IDate PlusDays(int days);
    }
}
