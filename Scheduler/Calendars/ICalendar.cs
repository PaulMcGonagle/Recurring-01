using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public interface ICalendar : IVertex
    {
        IEdgeVertexs<IDate> Dates { get; set; }
    }
}
