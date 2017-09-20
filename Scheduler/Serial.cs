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
            IEdgeRangeTime rangeTime, 
            string timeZoneProvider)
        {
            EdgeSchedule = new EdgeSchedule(schedule);
            RangeTime = rangeTime;
            TimeZoneProvider = timeZoneProvider;
        }

        [IgnoreDataMember]
        public IEdgeSchedule EdgeSchedule { get; set; }
        [IgnoreDataMember]
        public IEdgeRangeTime RangeTime;

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
                        From = DateTimeHelper.GetZonedDateTime(date, RangeTime.Range.From, TimeZoneProvider),
                        Period = RangeTime.Range?.Period,
                    }));

            return episodes;
        }

        private void Validate()
        {
            if (EdgeSchedule?.Schedule == null)
                throw new ArgumentException("Schedule");

            if (RangeTime == null)
                throw new ArgumentException("RangeTime");

            if (RangeTime?.Range?.Period == null)
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
            RangeTime?.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
