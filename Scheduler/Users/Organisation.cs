using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Organisation : Vertex
    {
        public string Title { get; set; }

        [IgnoreDataMember]
        public EdgeVertex<Location> Location { get; set; }

        [IgnoreDataMember]
        public List<Event> Events { get; set; }

        public override SaveResult Save(IArangoDatabase db)
        {
            var result = Save<Organisation>(db);

            if (result != SaveResult.Success)
                return result;

            if (Location != null)
                result = Location.Save(db, this);

            if (result != SaveResult.Success)
                return result;

            return SaveResult.Success;
        }
    }
}
