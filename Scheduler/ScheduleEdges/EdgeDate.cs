using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeDate : EdgeVertex<IDate>, IEdgeDate
    {
        public EdgeDate(IDate toVertex, string label = null)
            : base(toVertex, label)
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

        public IDate Date
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
