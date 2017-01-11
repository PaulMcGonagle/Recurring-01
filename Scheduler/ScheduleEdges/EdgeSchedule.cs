using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeSchedule : EdgeVertex<ISchedule>
    {
        public EdgeSchedule(ISchedule toVertex)
            : base(toVertex)
        {
            
        }

        public ISchedule Schedule => ToVertex;
    }
}
