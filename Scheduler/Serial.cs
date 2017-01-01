using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public class Serial : Vertex
    {
        [IgnoreDataMember]
        public EdgeSchedule EdgeSchedule;

        public LocalTime? From;
        public Period Period;
        public string TimeZoneProvider;

        [IgnoreDataMember]
        public IEnumerable<Episode> Episodes
        {
            get
            {
                if (EdgeSchedule == null || EdgeSchedule.Schedule == null)
                    throw new System.ArgumentException("Schedule");

                if (!From.HasValue)
                    throw new System.ArgumentException("From");

                if (Period == null)
                    throw new System.ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new System.ArgumentException("TimeZoneProvider");

                return EdgeSchedule.ToVertex.Dates.Select(o => new Episode
                {
                    From = DateTimeHelper.GetZonedDateTime(o, From.Value, TimeZoneProvider),
                    Period = Period,
                });
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            var result = Save<Serial>(db);

            if (result != SaveResult.Success)
                return result;

            return EdgeSchedule.Save(db, this);
        }
    }
}
