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
        IEnumerable<Episode> Episodes { get; }

        Vertex.SaveResult Save(IArangoDatabase db, IClock clock);
    }
}
