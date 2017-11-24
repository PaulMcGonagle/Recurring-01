using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRangeDate : EdgeVertex<IRangeDate>, IEdgeRangeDate
    {
        public EdgeRangeDate(IRangeDate toVertex, string label = null)
            : base(toVertex, null)
        {
            Guard.AgainstNull(RangeDate, nameof(RangeDate));
        }

        public EdgeRangeDate(IDate start, IDate end, string label = null)
            : this(new RangeDate.Builder
            {
                Start = start,
                End = end
            }.Build(), label)
        {
            
        }

        public EdgeRangeDate(LocalDate start, LocalDate end, string label = null)
            : this(start: new Date(start), end: new Date(end))
        {
            
        }

        public IRangeDate RangeDate
        {
            get => ToVertex;
            set => ToVertex = value;
        }

        public void Validate()
        {
        }
    }
}
