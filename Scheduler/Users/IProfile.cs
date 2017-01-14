using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public interface IProfile : IVertex
    {
        string Email { get; set; }
        string Forename { get; set; }
        string HomeTimeZoneProvider { get; set; }
        EdgeVertexs<IOrganisation> Organisations { get; set; }
        string Surname { get; set; }
    }
}