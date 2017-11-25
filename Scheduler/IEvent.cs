using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public interface IEvent : IVertex
    {
        IEdgeVertex<ILocation> Location { get; set; }
        IEdgeVertexs<ISerial> Serials { get; set; }
        string Title { get; set; }
        IEdgeVertex<IInstance> Instance { get; set; }
    }
}