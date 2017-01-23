using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Episodes : List<IEpisode>, IEpisodes
    {
        public Episodes(IEnumerable<IEpisode> episodes)
        {
            AddRange(episodes);
        }

        public Episodes()
        {
            
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            ForEach(e => e.Save(db, clock));
        }
    }
}
