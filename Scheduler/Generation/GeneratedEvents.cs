using System.Collections.Generic;

namespace Scheduler.Generation
{
    public class GeneratedEvents
    {
        private List<GeneratedEvent> _items;

        public IEnumerable<GeneratedEvent> Items
        {
            get
            {
                return _items;
            }
            set { _items = (List<GeneratedEvent>) value; }
        }
    }
}
