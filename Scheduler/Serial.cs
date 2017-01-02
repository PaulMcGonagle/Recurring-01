using System;
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
                    throw new ArgumentException("Schedule");

                if (!From.HasValue)
                    throw new ArgumentException("From");

                if (Period == null)
                    throw new ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new ArgumentException("TimeZoneProvider");

                return EdgeSchedule.ToVertex.Dates.Select(o => new Episode
                {
                    From = DateTimeHelper.GetZonedDateTime(o, From.Value, TimeZoneProvider),
                    Period = Period,
                });
            }
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Serial>(db),
                () => EdgeSchedule?.Save(db, clock, this) ?? SaveDummy(),
                () => base.Save(db, clock),
            });
        }
    }
}
