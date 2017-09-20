using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEdgeVertexs<IEpisode> GenerateEpisodes(IClock clock);

        IEdgeSchedule EdgeSchedule { get; set; }
    }
}
