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

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock)
        {
            foreach (var serial in this)
            {
                var result = serial.Save(db, clock);

                if (result != Vertex.SaveResult.Success)
                    return result;
            }

            return Vertex.SaveResult.Success;
        }
    }
}
