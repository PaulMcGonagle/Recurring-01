using System;
using System.Globalization;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Ranges
{
    public class RangeDate : Vertex, IRangeDate
    {
        [IgnoreDataMember]
        public IEdgeDate Start { get; set; }

        [IgnoreDataMember]
        public IEdgeDate End { get; set; }

        public override string ToString()
        {
            return $"{Start.Date}->{End.Date}";
        }

        public bool Contains(LocalDate localDate)
        {
            return Start.Date?.Value <= localDate && localDate <= End.Date?.Value;
        }

        public override void Validate()
        {
            Guard.AgainstNull(Start, nameof(Start));
            Guard.AgainstNull(End, nameof(End));

            if (Start.Date.Value > End.Date.Value)
                throw new ArgumentOutOfRangeException(nameof(Start),
                    $"Start date [{Start.Date.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than End date [{End.Date.Value.ToString("D", CultureInfo.CurrentCulture)}]");
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeDate>(db);
            Start?.Save(db, clock, this, "HasStartDate");
            End?.Save(db, clock, this, "HasEndDate");
            base.Save(db, clock);
        }

        public class Builder : Builder<RangeDate>
        {
            public IDate Start
            {
                set => EdgeStart = new EdgeDate(value);
            }

            public IEdgeDate EdgeStart
            {
                set => _target.Start = value;
            }

            public IDate End
            {
                set => _target.End = new EdgeDate(value);
            }

            public IEdgeDate EdgeEnd
            {
                set => _target.End = value;
            }
        }
    }
}