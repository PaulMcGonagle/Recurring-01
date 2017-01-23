using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISerials : IList<Serial>
    {
        IEpisodes Episodes { get; }

        void Save(IArangoDatabase db, IClock clock);
    }
}
