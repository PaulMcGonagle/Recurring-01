using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Serial : Vertex
    {
        [IgnoreDataMember]
        public Edge ScheduleEdge;

        [IgnoreDataMember]
        public Schedule Schedule
        {
            get { return (Schedule)ScheduleEdge.ToVertex; }
            set
            {
                if (ScheduleEdge == null)
                    ScheduleEdge = new Edge();

                ScheduleEdge.ToVertex = value;
            }
        }

        public LocalTime? From;
        public Period Period;
        public string TimeZoneProvider;

        [IgnoreDataMember]
        public IEnumerable<Episode> Episodes
        {
            get
            {
                if (ScheduleEdge == null || Schedule == null)
                    throw new System.ArgumentException("Schedule");

                if (!From.HasValue)
                    throw new System.ArgumentException("From");

                if (Period == null)
                    throw new System.ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new System.ArgumentException("TimeZoneProvider");

                return Schedule.Dates.Select(o => new Episode
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

            return ScheduleEdge.Save(db);
        }
    }
}
