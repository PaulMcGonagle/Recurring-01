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
