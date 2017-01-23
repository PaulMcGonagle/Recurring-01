using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface IEpisodes : IList<IEpisode>
    {
        void Save(IArangoDatabase db, IClock clock);

        void AddRange(IEnumerable<IEpisode> episodes);
    }
}
