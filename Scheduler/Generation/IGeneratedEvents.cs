using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public interface IGeneratedEvents : IList<IGeneratedEvent>
    {
        void Save(IArangoDatabase db, IClock clock);
    }
}
