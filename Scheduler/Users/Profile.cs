using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Profile : Vertex
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string HomeTimeZoneProvider { get; set; }

        public EdgeVertexs<Organisation> Organisations { get; set; }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save<Profile>(db);
        }
    }
}
