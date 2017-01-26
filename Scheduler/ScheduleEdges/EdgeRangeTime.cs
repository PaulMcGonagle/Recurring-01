using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRangeTime : EdgeVertex<ITimeRange>, IEdgeRangeTime
    {
        public EdgeRangeTime(ITimeRange timeRange)
            : base(timeRange)
        {

        }

        public EdgeRangeTime(
            LocalTime from,
            Period period)
            : this(new TimeRange(
                from: from,
                period: period))
        {

        }

        public ITimeRange Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
