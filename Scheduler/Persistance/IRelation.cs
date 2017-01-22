using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IRelation
    {
        string Label { get; set; }

        Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex);
    }
}