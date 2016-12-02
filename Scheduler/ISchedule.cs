using NodaTime;
using System.Collections.Generic;
using System.Xml.Linq;
using ArangoDB.Client;

namespace Scheduler
{
    public interface ISchedule : IPersistableEntry
    {
        IEnumerable<LocalDate> Dates { get; }
    }
}
