using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeRange : EdgeVertex<Range>
    {
        public EdgeRange(Range toVertex)
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
            : this(new Range(
                fromYear: fromYear,
                fromMonth: fromMonth,
                fromDay: fromDay,
                toYear: toYear,
                toMonth: toMonth,
                toDay: toDay))
        {

        }

        public Range Range
        {
            get { return ToVertex; }
            set { ToVertex = value; }
        }
    }
}
