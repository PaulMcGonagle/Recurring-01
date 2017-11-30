using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public class Serial : Vertex, ISerial
    {
        public Serial()
        {

        }

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

        [IgnoreDataMember] public IEdgeRangeTime RangeTime;

        public string TimeZoneProvider;

        public IEdgeVertexs<IEpisode> GenerateEpisodes(IClock clock)
        {
            Validate();

            var episodes = new EdgeVertexs<IEpisode>();

            var dates = EdgeSchedule.Schedule
                .Generate(clock);

            episodes.AddRange(
                dates
                    .Select(date => new Episode.Builder
                    {
                        SourceSerial = this,
                        SourceGeneratedDate = date,
                        Start = DateTimeHelper.GetZonedDateTime(date, RangeTime.Range.Start, TimeZoneProvider),
                        Period = RangeTime.Range?.Period,
                    }.Build()));

            return episodes;
        }

        public override void Validate()
        {
            Guard.AgainstNull(EdgeSchedule, nameof(EdgeSchedule));
            Guard.AgainstNull(RangeTime, nameof(RangeTime));
            Guard.AgainstNull(TimeZoneProvider, nameof(TimeZoneProvider));
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

        public class Builder : Vertex.Builder<Serial>
        {
            public IEdgeSchedule EdgeSchedule 
            {
                set => _target.EdgeSchedule = value;
            }

            public IEdgeRangeTime RangeTime
            {
                set => _target.RangeTime = value;
            }

            public string TimeZoneProvider
            {
                set => _target.TimeZoneProvider = value;
            }
        }
    }
}