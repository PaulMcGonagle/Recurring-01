using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Location : Vertex, ILocation
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }

        [IgnoreDataMember]
        public Serials Serials { get; set; }

        protected override IEnumerable<IVertex> Links
        {
            get
            {
                if (Serials != null)
                    foreach (var serial in Serials)
                        yield return serial;
            }
        }

        public override void Validate()
        {
            base.Validate();

            if ((!string.IsNullOrWhiteSpace(Latitude) && string.IsNullOrWhiteSpace(Longitude))
            || (string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longitude)))
            {
                throw new ArgumentException("Both Longitude and Latitude must be provided");
            }

            if (string.IsNullOrWhiteSpace(Address) && string.IsNullOrWhiteSpace(Latitude))
            {
                throw new ArgumentException("Either Location Address or Latitude/Longitude must be provided");
            }
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Location>(db);
            base.Save(db, clock);

        }

        public class Builder : Builder<Location>
        {
            public string Address
            {
                set => _target.Address = value;
            }

            public string Latitude
            {
                set => _target.Latitude = value;
            }

            public string Longitude
            {
                set => _target.Longitude = value;
            }

            public Serials Serials
            {
                set => _target.Serials = value;
            }
        }
    }
}
