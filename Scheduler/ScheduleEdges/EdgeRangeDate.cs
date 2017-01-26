using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRangeDate : EdgeVertex<IDateRange>, IEdgeRangeDate
    {
        public EdgeRangeDate(DateRange toVertex)
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
            : this(new DateRange(
                fromYear: fromYear,
                fromMonth: fromMonth,
                fromDay: fromDay,
                toYear: toYear,
                toMonth: toMonth,
                toDay: toDay))
        {

        }

        public IDateRange Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
