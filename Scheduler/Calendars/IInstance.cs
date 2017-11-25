using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public interface IInstance : IVertex
    {
        IEdgeVertexs<IEpisode> Episodes { get; set; }
        Instant Time { get; set; }
    }
}