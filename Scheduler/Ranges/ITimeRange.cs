using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public interface ITimeRange : IVertex
    {
        LocalTime From { get; set; }
        Period Period { get; set; }
    }
}