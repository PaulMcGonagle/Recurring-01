using System.Collections.Generic;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Organisation : Vertex
    {
        public string Title { get; set; }

        public List<Event> Events { get; set; }

        public SaveResult Save(IArangoDatabase db)
        {
            return Save<Organisation>(db);
        }
    }
}
