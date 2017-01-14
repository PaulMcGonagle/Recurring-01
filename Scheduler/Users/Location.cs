using System;
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

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
                       {
                () => Save<Location>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
