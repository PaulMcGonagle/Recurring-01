using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class EdgeVertexs<T> : List<IEdgeVertex<T>>, IEdgeVertexs<T> where T : IVertex
    {
        public EdgeVertexs()
        {
            
        }

        public EdgeVertexs(IVertex toVertex)
        {
            var t = new EdgeVertex<IVertex>(toVertex: toVertex);

            Add((IEdgeVertex<T>)t);
        }

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            foreach (var edge in this)
            {
                edge.Save(db, clock, fromVertex);
            }

            return Vertex.SaveResult.Success;
        }

        public void SetToDelete()
        {
            this.ForEach(e => e.ToVertex.SetToDelete());

        }

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock)
        {
            throw new NotImplementedException();
        }
    }
}
