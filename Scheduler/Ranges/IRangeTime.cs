using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public interface IRangeTime : IVertex
    {
        LocalTime From { get; set; }
        Period Period { get; set; }
    }
}