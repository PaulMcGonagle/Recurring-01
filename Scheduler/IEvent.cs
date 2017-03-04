using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Users;

namespace Scheduler
{
    public interface IEvent : IVertex
    {
        EdgeVertex<Location> Location { get; set; }
        IEdgeVertexs<ISerial> Serials { get; set; }
        string Title { get; set; }
        IEdgeVertex<IInstance> Instance { get; set; }
        IEdgeVertexs<ITag> Tags { get; set; }
    }
}