using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Episodes : List<IEpisode>, IEpisodes
    {
        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock)
        {
            foreach (var episode in this)
            {
                var result = episode.Save(db, clock);

                if (result != Vertex.SaveResult.Success)
                    return result;
            }

            return Vertex.SaveResult.Success;
        }
    }
}
