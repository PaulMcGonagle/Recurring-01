using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public interface ICalendar : IVertex
    {
        IEdgeVertexs<IEpisode> Episodes { get; set; }
    }
}
