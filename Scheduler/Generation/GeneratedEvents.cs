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

        public Vertex.SaveResult Save(IArangoDatabase db, IClock clock)
        {
            foreach (var generatedEvent in this)
            {
                var result = generatedEvent.Save(db, clock);

                if (result != Vertex.SaveResult.Success)
                    return result;
            }

            return Vertex.SaveResult.Success;
        }
    }
}
