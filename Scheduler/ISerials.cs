using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public interface ISerials : IList<Serial>
    {
        IEpisodes Episodes { get; }

        void Save(IArangoDatabase db, IClock clock);
    }
}
