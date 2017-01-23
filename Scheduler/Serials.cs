using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerials
    {
        public IEpisodes Episodes
        {
            get
            {
                var episodes = new Episodes();

                episodes.AddRange(
                    this.SelectMany(ce => ce.Episodes));

                return episodes;
            }
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            foreach (var serial in this)
            {
                serial.Save(db, clock);
            }

            return;
        }
    }
}
