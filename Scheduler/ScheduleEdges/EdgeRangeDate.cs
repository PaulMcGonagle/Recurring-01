using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRangeDate : EdgeVertex<IRangeDate>, IEdgeRangeDate
    {
        public EdgeRangeDate(IRangeDate toVertex)
            : base(toVertex)
        {

        }

        public EdgeRangeDate(
            int fromYear, 
            YearMonth.MonthValue fromMonth, 
            int fromDay, 
            int toYear,
            YearMonth.MonthValue toMonth,
            int toDay)
            : this(new RangeDate(
                fromYear: fromYear,
                fromMonth: fromMonth,
                fromDay: fromDay,
                toYear: toYear,
                toMonth: toMonth,
                toDay: toDay))
        {

        }

        public IRangeDate Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
