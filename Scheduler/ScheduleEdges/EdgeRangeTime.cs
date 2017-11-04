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
            LocalTime start,
            Period period)
            : this(new RangeTime(
                start: start,
                period: period))
        {

        }

        public IRangeTime Range
        {
            get => ToVertex;
            set => ToVertex = value;
        }
    }
}
