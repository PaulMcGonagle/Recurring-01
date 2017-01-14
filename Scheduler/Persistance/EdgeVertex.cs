using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class EdgeVertex<T> : IEdgeVertex<T> where T : IVertex
    {
        public EdgeVertex(T toVertex)
        {
            ToVertex = toVertex;
        }

        public Edge Edge { get; set; }

        public T ToVertex {
            get { return (T) Edge.ToVertex; }
            set
            {
                if (Edge == null) Edge = new Edge();

                Edge.ToVertex = value;
            }
        }

        #region Save

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            return Edge.Save(db, clock, fromVertex);
        }

        #endregion
    }
}
