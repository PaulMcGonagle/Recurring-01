using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
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
