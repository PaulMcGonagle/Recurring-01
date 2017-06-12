using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public interface ISerials : IList<Serial>
    {
        IEpisodes GenerateEpisodes(IClock clock);

        void Save(IArangoDatabase db, IClock clock);
    }
}
