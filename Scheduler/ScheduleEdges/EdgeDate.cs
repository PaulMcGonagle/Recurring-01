using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeDate : EdgeVertex<Date>
    {
        public EdgeDate(Date toVertex)
            : base(toVertex)
        {

        }

        public EdgeDate(int year, YearMonth.MonthValue month, int day)
            : this(new Date(year, month, day))
        {
        }

        public EdgeDate(LocalDate localDate)
            : this(new Date(localDate))
        {
        }

        public Date Date
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
