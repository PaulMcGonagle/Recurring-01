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

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Location>(db);
            base.Save(db, clock);
        }
    }
}
