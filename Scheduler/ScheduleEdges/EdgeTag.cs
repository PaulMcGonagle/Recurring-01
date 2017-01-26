using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeTag : EdgeVertex<ITag>, IEdgeTag
    {
        public EdgeTag(ITag toVertex)
            : base(toVertex)
        {
            
        }

        public ITag Tag
        {
            get { return ToVertex; }
            set { ToVertex = value;  }
        }
    }
}
