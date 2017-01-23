using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public class GeneratedEvents : List<IGeneratedEvent>
    {
        public GeneratedEvents() { }

        public GeneratedEvents(IEnumerable<IGeneratedEvent> generatedEvents)
        {
            AddRange(generatedEvents);
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            this.ForEach(e => e.Save(db, clock));
        }
    }
}
