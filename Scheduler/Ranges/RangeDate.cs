using System;
using System.Globalization;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public class RangeDate : Vertex, IRangeDate
    {
        [IgnoreDataMember]
        public EdgeDate Start { get; set; }
        [IgnoreDataMember]
        public EdgeDate End { get; set; }

        public void Validate()
        {
            if (Start?.Date?.Value != null && End?.Date?.Value == null && Start.Date?.Value <= End?.Date?.Value)
            {
                throw new ArgumentOutOfRangeException($"Range is invalid: Start={Start?.Date?.Value}, End={End?.Date?.Value}");
            }
        }

        public override string ToString()
        {
            return $"{Start.Date}->{End.Date}";
        }

        public bool Contains(LocalDate localDate)
        {
            return Start.Date?.Value <= localDate && localDate <= End.Date?.Value;
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeDate>(db);
            Start?.Save(db, clock, this, "HasStartDate");
            End?.Save(db, clock, this, "HasEndDate");
            base.Save(db, clock);
        }
    }

    public class RangeDateBuilder
    {
        private RangeDate _rangeDate;

        public RangeDateBuilder()
        {
            _rangeDate = new RangeDate();
        }

        public IDate Start
        {
            set => _rangeDate.Start = new EdgeDate(value);
        }

        public IDate End
        {
            set => _rangeDate.End = new EdgeDate(value);
        }

        public RangeDate Build()
        {
            if (_rangeDate.Start.Date.Value > _rangeDate.End.Date.Value)
                throw new ArgumentOutOfRangeException(nameof(_rangeDate.Start), $"Start date [{_rangeDate.End.Date.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{_rangeDate.Start.Date.Value.ToString("D", CultureInfo.CurrentCulture)}]");

            return _rangeDate;
        }
    }
}
