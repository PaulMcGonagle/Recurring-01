using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Generation
{
    public interface IGeneratedDate : IVertex
    {
        Date Date { get; set; }

        EdgeSchedule Source { get; set; }
    }
}
