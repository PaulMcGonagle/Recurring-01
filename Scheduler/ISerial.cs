using System.Runtime.Serialization;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEdgeVertexs<IEpisode> Episodes { get; }

        IEdgeSchedule EdgeSchedule { get; set; }
    }
}
