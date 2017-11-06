using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeTag : EdgeVertex<ITag>, IEdgeTag
    {
        public EdgeTag(ITag toVertex, string label = null)
            : base(toVertex, label)
        {
            
        }

        public ITag Tag
        {
            get { return ToVertex; }
            set { ToVertex = value;  }
        }
    }
}
