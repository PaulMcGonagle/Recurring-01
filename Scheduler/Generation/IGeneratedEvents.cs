using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public interface IGeneratedEvents : IList<IGeneratedEvent>
    {
        Vertex.SaveResult Save(IArangoDatabase db, IClock clock);
    }
}
