using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerials
    {
        public IEnumerable<IEpisode> GenerateEpisodes(IClock clock)
        {
            return this
                .SelectMany(ce => ce.GenerateEpisodes(clock));
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            foreach (var serial in this)
            {
                serial.Save(db, clock);
            }
        }
    }
}
