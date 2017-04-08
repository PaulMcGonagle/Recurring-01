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
        public Serial(
            ISchedule schedule, 
            IEdgeRangeTime timeRange, 
            string timeZoneProvider)
        {
            EdgeSchedule = new EdgeSchedule(schedule);
            TimeRange = timeRange;
            TimeZoneProvider = timeZoneProvider;
        }

        [IgnoreDataMember]
        public IEdgeSchedule EdgeSchedule;
        [IgnoreDataMember]
        public IEdgeRangeTime TimeRange;
        [IgnoreDataMember]
        public IEdgeVertexs<ITag> Tags { get; set; }

        public string TimeZoneProvider;

        [IgnoreDataMember]
        public IEdgeVertexs<IEpisode> Episodes
        {
            get
            {
                if (EdgeSchedule?.Schedule == null)
                    throw new ArgumentException("Schedule");

                if (TimeRange == null)
                    throw new ArgumentException("TimeRange");

                if (TimeRange?.Range?.Period == null)
                    throw new ArgumentException("Period");

                if (TimeZoneProvider == null)
                    throw new ArgumentException("TimeZoneProvider");

                var episodes = new EdgeVertexs<IEpisode>();

                episodes.AddRange(
                    EdgeSchedule.Schedule
                        .Generate()
                        .Select(date => new Episode
                        {
                            SourceSerial = new EdgeVertex<ISerial>(this),
                            SourceGeneratedDate = new EdgeVertex<IDate>(date),
                            From = DateTimeHelper.GetZonedDateTime(date, TimeRange.Range.From, TimeZoneProvider),
                            Period = TimeRange.Range?.Period,
                        }));

                return episodes;
            }
        }

        protected override IEnumerable<IVertex> Links
        {
            get
            {
                var list = new List<IVertex>();

                list.AddRange(Episodes.Select(e => e.ToVertex));

                if (EdgeSchedule != null)
                {
                    list.Add(EdgeSchedule.Edge);
                    list.Add(EdgeSchedule.Schedule);
                }

                return list;
            }
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Serial>(db);
            EdgeSchedule?.Save(db, clock, this);
            Episodes?.Save(db, clock, this);
            Tags?.Save(db, clock, this);
            TimeRange?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
