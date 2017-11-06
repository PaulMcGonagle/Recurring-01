using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeSchedule : EdgeVertex<ISchedule>, IEdgeSchedule
    {
        public EdgeSchedule(ISchedule toVertex, string label = null)
            : base(toVertex, label)
        {
            
        }

        public ISchedule Schedule => ToVertex;
    }
}
