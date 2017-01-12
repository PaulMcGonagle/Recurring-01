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
    public class Serial : Vertex, ISerial
    {
        public Serial(ISchedule schedule)
        {
            EdgeSchedule = new EdgeSchedule(schedule);
        }

        [IgnoreDataMember]
        public EdgeSchedule EdgeSchedule;

        public LocalTime? From;
        public Period Period;
        public string TimeZoneProvider;

        [IgnoreDataMember]
        public IEpisodes Episodes
        {
            get
            {
                if (EdgeSchedule?.Schedule == null)
                    throw new ArgumentException("Schedule");

                if (!From.HasValue)
                    throw new ArgumentException("From");

                if (Period == null)
                    throw new ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new ArgumentException("TimeZoneProvider");

                var episodes = new Episodes();

                episodes.AddRange(
                    EdgeSchedule.ToVertex
                        .Generate()
                        .Select(o => new Episode
                        {
                            From = DateTimeHelper.GetZonedDateTime(o.Date, From.Value, TimeZoneProvider),
                            Period = Period,
                        }));

                return episodes;
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
