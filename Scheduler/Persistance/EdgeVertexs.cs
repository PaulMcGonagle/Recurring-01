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

        public EdgeVertexs(IEnumerable<IEdgeVertex<T>> edgeVertexs)
            : base(edgeVertexs)
        {
            
        }

        public EdgeVertexs(T toVertex)
        {
            Add(new EdgeVertex<T>(toVertex: toVertex));
        }

        public EdgeVertexs(IEnumerable<T> toVertexs)
            : this(toVertexs.Select(t => new EdgeVertex<T>(toVertex: t)))
        {

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
