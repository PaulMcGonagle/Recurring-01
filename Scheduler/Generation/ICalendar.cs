using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public interface ICalendar : IVertex
    {
        IEdgeVertexs<IDate> Dates { get; set; }
    }
}
