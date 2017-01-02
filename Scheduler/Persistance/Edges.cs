using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class Edges : List<Edge>
    {
        #region Save

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            foreach (var edge in this)
            {
                edge.Save(db, clock, fromVertex);
            }

            return Vertex.SaveResult.Success;
        }

        #endregion
    }
}
