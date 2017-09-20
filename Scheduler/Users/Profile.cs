using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class Profile : Vertex, IProfile
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string HomeTimeZoneProvider { get; set; }

        public EdgeVertexs<IOrganisation> Organisations { get; set; }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Profile>(db);
            Organisations?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
