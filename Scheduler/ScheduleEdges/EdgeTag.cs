using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeTag : EdgeVertex<Tag>
    {
        public EdgeTag(Tag toVertex)
            : base(toVertex)
        {
            
        }

        public Tag Tag
        {
            get { return ToVertex; }
            set { ToVertex = value;  }
        }
    }
}
