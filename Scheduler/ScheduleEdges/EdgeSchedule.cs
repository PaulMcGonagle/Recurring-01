using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeSchedule : EdgeVertex<ISchedule>, IEdgeSchedule
    {
        public EdgeSchedule(ISchedule toVertex)
            : base(toVertex)
        {
            
        }

        public ISchedule Schedule => ToVertex;
    }
}
