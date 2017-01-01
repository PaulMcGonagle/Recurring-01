using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Location : Vertex
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }

        [IgnoreDataMember]
        public Scheduler.Serials Serials { get; set; }

        public override SaveResult Save(IArangoDatabase db)
        {
            var result = Save<Location>(db);

            if (result != SaveResult.Success)
                return result;

            return SaveResult.Success;
        }
    }
}
