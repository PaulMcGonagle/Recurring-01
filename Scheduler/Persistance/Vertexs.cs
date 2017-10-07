using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public static class Vertexs
    {
        public static void Save(this IEnumerable<IVertex> vertexs, IArangoDatabase db, IClock clock)
        {
            foreach (var vertex in vertexs)
            {
                vertex.Save(db, clock);
            }
        }
    }
}
