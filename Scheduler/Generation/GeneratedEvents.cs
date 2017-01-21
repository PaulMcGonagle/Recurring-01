using System.Collections.Generic;

namespace Scheduler.Generation
{
    public class GeneratedEvents : List<IGeneratedEvent>
    {
        public GeneratedEvents() { }

        public GeneratedEvents(IEnumerable<IGeneratedEvent> generatedEvents)
        {
            AddRange(generatedEvents);
        }
    }
}
