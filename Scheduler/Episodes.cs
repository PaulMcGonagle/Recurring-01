using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Episodes : List<IEpisode>, IEpisodes
    {
        public Episodes(IEnumerable<IEpisode> episodes)
        {
            this.AddRange(episodes);
        }

        public Episodes()
        {
            
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            this.ForEach(e => e.Save(db, clock));
        }
    }
}
