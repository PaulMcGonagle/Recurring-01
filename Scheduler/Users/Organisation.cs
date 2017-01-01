using System;
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
            return Save(new Func<SaveResult>[]
                       {
                () => Save<Organisation>(db),
                () => Location?.Save(db, this) ?? SaveDummy(), 
                () => base.Save(db),
            });
        }
    }
}
