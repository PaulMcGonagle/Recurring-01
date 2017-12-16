using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEnumerable<IEpisode> GenerateEpisodes(IClock clock);

        IEdgeSchedule EdgeSchedule { get; set; }

        IEdgeRangeTime RangeTime { get; set; }

        string TimeZoneProvider { get; set; }
    }
}
