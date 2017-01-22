using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public class Serial : Vertex, ISerial
    {
        public Serial(
            ISchedule schedule, 
            ITimeRange timeRange, 
            string timeZoneProvider)
        {
            EdgeSchedule = new EdgeSchedule(schedule);
            TimeRange = timeRange;
            TimeZoneProvider = timeZoneProvider;
        }

        [IgnoreDataMember]
        public EdgeSchedule EdgeSchedule;

        public ITimeRange TimeRange;
        public string TimeZoneProvider;

        [IgnoreDataMember]
        public IEpisodes Episodes
        {
            get
            {
                if (EdgeSchedule?.Schedule == null)
                    throw new ArgumentException("Schedule");

                if (TimeRange == null)
                    throw new ArgumentException("TimeRange");

                if (TimeRange?.Period == null)
                    throw new ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new ArgumentException("TimeZoneProvider");

                var episodes = new Episodes();

                episodes.AddRange(
                    EdgeSchedule.ToVertex
                        .Generate()
                        .Select(o => new Episode
                        {
                            SourceSerial = new EdgeVertex<ISerial>(this),
                            SourceGeneratedDate = new EdgeVertex<IGeneratedDate>(o),
                            From = DateTimeHelper.GetZonedDateTime(o.Date, TimeRange.From, TimeZoneProvider),
                            Period = TimeRange.Period,
                        }));

                return episodes;
            }
        }

        protected override IEnumerable<IVertex> Links
        {
            get
            {
                var list = new List<IVertex>();

                list.AddRange(Episodes);

                if (EdgeSchedule != null)
                {
                    list.Add(EdgeSchedule.Edge);
                    list.Add(EdgeSchedule.ToVertex);
                }

                return list;
            }
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Serial>(db),
                () => EdgeSchedule?.Save(db, clock, this) ?? SaveDummy(),
                () => Episodes?.Save(db, clock) ?? SaveDummy(),
                () => base.Save(db, clock),
            });
        }
    }
}
