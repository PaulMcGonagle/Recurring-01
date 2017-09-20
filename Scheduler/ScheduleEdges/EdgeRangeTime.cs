using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRangeTime : EdgeVertex<IRangeTime>, IEdgeRangeTime
    {
        public EdgeRangeTime(IRangeTime rangeTime)
            : base(rangeTime)
        {

        }

        public EdgeRangeTime(
            LocalTime from,
            Period period)
            : this(new RangeTime(
                from: from,
                period: period))
        {

        }

        public IRangeTime Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
