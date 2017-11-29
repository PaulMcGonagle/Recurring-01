using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
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

        public override void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Title, nameof(Title));
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Organisation>(db);
            Location?.Save(db, clock, this);
            base.Save(db, clock);
        }

        public class Builder : Vertex.Builder<Organisation>
        {
            public string Title
            {
                set => _target.Title = value;
            }

            public Location Location
            {
                set => _target.Location = new EdgeVertex<Location>(value);
            }
        }
    }
}
