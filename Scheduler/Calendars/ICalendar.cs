using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public interface ICalendar : IVertex
    {
        IEdgeVertexs<IEvent> Events { get; set; }
    }
}
