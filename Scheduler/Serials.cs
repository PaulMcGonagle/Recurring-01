using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerials
    {
        public IEpisodes GenerateEpisodes(IClock clock)
        {
            var episodes = new Episodes();

            episodes.AddRange(this
                .SelectMany(ce => ce.GenerateEpisodes(clock))
                .Select(e => e.ToVertex));

            return episodes;
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
