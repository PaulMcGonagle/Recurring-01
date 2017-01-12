using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IEpisode
    {
        ZonedDateTime From { get; set; }
        Period Period { get; set; }
        EdgeVertex<IGeneratedDate> SourceGeneratedDate { get; set; }
        EdgeVertex<ISerial> SourceSerial { get; set; }
        ZonedDateTime To { get; }

        Vertex.SaveResult Save(IArangoDatabase db, IClock clock);
        string ToString();
    }
}