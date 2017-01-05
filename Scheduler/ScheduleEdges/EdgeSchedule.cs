using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler.ScheduleEdges
{
    public class EdgeSchedule : EdgeVertex<ISchedule>
    {
        public EdgeSchedule(ISchedule toVertex)
            : base(toVertex)
        {
            
        }

        public ISchedule Schedule => ToVertex;
    }
}
