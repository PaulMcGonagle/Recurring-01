using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace Scheduler.Persistance
{
    public class Edges : List<Edge>
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
