using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IEdgeVertex<T> where T : IVertex
    {
        IEdge Edge { get; set; }
        T ToVertex { get; set; }

        void Save(IArangoDatabase db, IClock clock, IVertex fromVertex, string label = null);
    }
}