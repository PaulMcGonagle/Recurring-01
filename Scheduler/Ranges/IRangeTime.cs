using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public interface IRangeTime : IVertex
    {
        LocalTime Start { get; set; }
        Period Period { get; set; }
        LocalTime End { get; }
    }
}