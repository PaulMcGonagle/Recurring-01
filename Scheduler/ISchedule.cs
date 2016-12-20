using NodaTime;
using System.Collections.Generic;
using System.Xml.Linq;
using ArangoDB.Client;

namespace Scheduler
{
    public interface ISchedule
    {
        IEnumerable<Date> Dates { get; }
    }
}
