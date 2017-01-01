using ArangoDB.Client;

namespace Scheduler.Persistance
{
    public class EdgeVertex<T> where T : Vertex
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

        public Vertex.SaveResult Save(IArangoDatabase db, Vertex fromVertex)
        {
            return Edge.Save(db, fromVertex);
        }

        #endregion
    }
}
