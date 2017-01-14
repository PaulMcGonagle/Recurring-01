using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public interface ILocation : IVertex
    {
        string Address { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
    }
}