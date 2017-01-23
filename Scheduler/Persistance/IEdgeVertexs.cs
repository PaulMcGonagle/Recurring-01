using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public interface IEdgeVertexs<T> : IList<IEdgeVertex<T>> where T : IVertex
    {
        void Save(IArangoDatabase db, IClock clock, Vertex fromVertex);
    }
}