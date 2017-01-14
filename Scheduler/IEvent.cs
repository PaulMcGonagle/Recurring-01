using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public interface IEvent : IVertex
    {
        EdgeVertex<Location> Location { get; set; }
        ISerials Serials { get; set; }
        string Title { get; set; }
    }
}