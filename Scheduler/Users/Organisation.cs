using System.Collections.Generic;
using ArangoDB.Client;

namespace Scheduler.Users
{
    public class Organisation : PersistableEntity
    {
        public string Title { get; set; }

        public List<Event> Events { get; set; }

        public void Save(IArangoDatabase db)
        {
            Save<Organisation>(db);
        }
    }
}
