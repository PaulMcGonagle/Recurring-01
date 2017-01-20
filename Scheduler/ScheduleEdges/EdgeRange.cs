using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRange : EdgeVertex<DateRange>
    {
        public EdgeRange(DateRange toVertex)
            : base(toVertex)
        {

        }

        public EdgeRange(
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

        public DateRange Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
