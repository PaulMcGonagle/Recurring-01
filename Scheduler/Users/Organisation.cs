﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Organisation : Vertex, IOrganisation
    {
        public string Title { get; set; }

        [IgnoreDataMember]
        public EdgeVertex<Location> Location { get; set; }

        [IgnoreDataMember]
        public List<Event> Events { get; set; }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Organisation>(db);
            Location?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
