using System;
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
        public IEdgeSchedule EdgeSchedule { get; set; }
        [IgnoreDataMember]
        public IEdgeRangeTime TimeRange;

        public string TimeZoneProvider;

        public IEdgeVertexs<IEpisode> GenerateEpisodes(IClock clock)
        {
            Validate();

            var episodes = new EdgeVertexs<IEpisode>();

            episodes.AddRange(
                EdgeSchedule.Schedule
                    .Generate(clock)
                    .Select(date => new Episode
                    {
                        SourceSerial = new EdgeVertex<ISerial>(this),
                        SourceGeneratedDate = new EdgeVertex<IDate>(date),
                        From = DateTimeHelper.GetZonedDateTime(date, TimeRange.Range.From, TimeZoneProvider),
                        Period = TimeRange.Range?.Period,
                    }));

            return episodes;
        }

        private void Validate()
        {
            if (EdgeSchedule?.Schedule == null)
                throw new ArgumentException("Schedule");

            if (TimeRange == null)
                throw new ArgumentException("RangeTime");

            if (TimeRange?.Range?.Period == null)
                throw new ArgumentException("Period");

            if (TimeZoneProvider == null)
                throw new ArgumentException("TimeZoneProvider");
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Validate();

            Save<Serial>(db);
            EdgeSchedule?.Save(db, clock, this);
            Tags?.Save(db, clock, this);
            TimeRange?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
