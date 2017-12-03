using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IEpisode : IVertex
    {
        ZonedDateTime Start { get; set; }
        Period Period { get; set; }
        EdgeVertex<IDate> SourceGeneratedDate { get; set; }
        EdgeVertex<ISerial> SourceSerial { get; set; }
        ZonedDateTime End { get; }
        string ToString();
    }
}