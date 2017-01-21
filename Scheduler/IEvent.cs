using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public interface IEvent : IVertex
    {
        EdgeVertex<Location> Location { get; set; }
        IEdgeVertexs<ISerial> Serials { get; set; }
        string Title { get; set; }
    }
}