using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler
{
    public class Serial : Vertex, ISerial
    {
        [IgnoreDataMember]
        public IEdgeSchedule EdgeSchedule { get; set; }

        [IgnoreDataMember]
        public IEdgeRangeTime RangeTime { get; set; }

        public string TimeZoneProvider { get; set; }

        public IEnumerable<IEpisode> GenerateEpisodes(IClock clock)
        {
            Validate();

            var episodes = new List<IEpisode>();

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

        public class Builder : Builder<Serial>
        {
            public IEdgeSchedule EdgeSchedule 
            {
                set => _target.EdgeSchedule = value;
            }

            public ISchedule Schedule
            {
                set => EdgeSchedule = new EdgeSchedule(value);
            }

            public IRangeTime RangeTime
            {
                set => _target.RangeTime = new EdgeRangeTime(value);
            }

            public string TimeZoneProvider
            {
                set => _target.TimeZoneProvider = value;
            }
        }
    }
}