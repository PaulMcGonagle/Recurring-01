using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace Scheduler.Persistance
{
    public class EdgeVertexs<T> : List<EdgeVertex<T>> where T : Vertex
    {
        public Vertex.SaveResult Save(IArangoDatabase db, Vertex fromVertex)
        {
            foreach (var edge in this)
            {
                edge.Save(db, fromVertex);
            }

            return Vertex.SaveResult.Success;
        }
    }
}
