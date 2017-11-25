using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Calendars
{
    public interface IInstances : IList<IInstance>
    {
        void Save(IArangoDatabase db, IClock clock);
    }
}
