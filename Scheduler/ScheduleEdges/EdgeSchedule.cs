using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeSchedule : EdgeVertex<Schedule>
    {
        public EdgeSchedule(Schedule toVertex)
            : base(toVertex)
        {
            
        }

        public Schedule Schedule => ToVertex;
    }
}
