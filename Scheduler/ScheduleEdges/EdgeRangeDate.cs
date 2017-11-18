using System.IO;
using Neo4jClient.Cypher;
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

        }

        public EdgeRangeDate(
            int fromYear, 
            YearMonth.MonthValue fromMonth, 
            int fromDay, 
            int toYear,
            YearMonth.MonthValue toMonth,
            int toDay)
            : this(
                start: new Date(year: fromYear, month: fromMonth, day: fromDay),
                end: new Date(year: toYear, month: toMonth, day: toDay)
            )
        {

        }

        public EdgeRangeDate(
            IDate start,
            IDate end)
            : this(new RangeDateBuilder
            {
                Start = start,
                End = end
            }.Build())
        {

        }

        public EdgeRangeDate(
            LocalDate start,
            LocalDate end)
            : this(
                start: new Date(start),
                end: new Date(end))
        {

        }

        public IRangeDate Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
