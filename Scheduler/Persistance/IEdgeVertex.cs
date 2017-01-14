using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IEdgeVertex<T> where T : IVertex
    {
        Edge Edge { get; set; }
        T ToVertex { get; set; }

        Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex);
    }
}