using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class Edges : List<Edge>
    {
        #region Save

        public void Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            foreach (var edge in this)
            {
                edge.Save(db, clock, fromVertex);
            }

            return;
        }

        #endregion
    }
}
