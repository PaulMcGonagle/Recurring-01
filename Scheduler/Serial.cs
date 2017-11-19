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
                        Start = DateTimeHelper.GetZonedDateTime(date, RangeTime.Range.Start, TimeZoneProvider),
                        Period = RangeTime.Range?.Period,
                    }));

            return episodes;
        }

        public void Validate()
        {
            if (EdgeSchedule == null)
                throw new ArgumentNullException(nameof(EdgeSchedule));

            if (RangeTime == null)
                throw new ArgumentNullException(nameof(RangeTime));

            if (TimeZoneProvider == null)
                throw new ArgumentNullException(nameof(TimeZoneProvider));
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

    public class SerialBuilder
    {
        private readonly Serial _serial;

        public SerialBuilder()
        {
            _serial = new Serial();
        }

        public IEdgeSchedule EdgeSchedule { set => _serial.EdgeSchedule = value; }

        public IEdgeRangeTime RangeTime
        {
            set => _serial.RangeTime = value;
        }

        public string TimeZoneProvider
        {
            set => _serial.TimeZoneProvider = value;
        }

        public Serial Build()
        {
            _serial.Validate();

            return _serial;
        }
    }
}
