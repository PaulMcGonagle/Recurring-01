using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IEpisode : IVertex
    {
        ZonedDateTime From { get; set; }
        Period Period { get; set; }
        EdgeVertex<IDate> SourceGeneratedDate { get; set; }
        EdgeVertex<ISerial> SourceSerial { get; set; }
        ZonedDateTime To { get; }
        string ToString();
    }
}