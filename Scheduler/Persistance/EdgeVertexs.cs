using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class EdgeVertexs<T> : List<EdgeVertex<T>> where T : Vertex
    {
        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            foreach (var edge in this)
            {
                edge.Save(db, clock, fromVertex);
            }

            return Vertex.SaveResult.Success;
        }
    }
}
