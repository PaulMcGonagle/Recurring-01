using System;
using System.Globalization;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Ranges
{
    public class RangeTime : Vertex, IRangeTime
    {
        public LocalTime Start { get; set; }

        public Period Period { get; set; }

        [IgnoreDataMember]
        public LocalTime End => Start.Plus(Period);

        public override void Validate()
        {
            Guard.AgainstNullOrDefault(Start, nameof(Start));
            Guard.AgainstNull(Period, nameof(Period));

            var duration = Period.ToDuration().Ticks;

            if (duration < 0)
                throw new ArgumentOutOfRangeException(nameof(Period),
                    $"Period duration cannot be negative. Period: {Period.ToString()}, Dduration.Ticks: {duration})");
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeTime>(db);
            base.Save(db, clock);
        }

        public class Builder : Vertex.Builder<RangeTime>
        {
            private LocalTime? _endTimeSupplied;

            public LocalTime Start
            {
                set => _target.Start = value;
            }

            public LocalTime End
            {
                set
                {
                    _endTimeSupplied = value;
                    _target.Period = default(Period);
                }
            }

            public Period Period
            {
                set
                {
                    _target.Period = value;
                    _endTimeSupplied = null;
                }
            }

            public RangeTime Build()
            {
                if (_target.Period == null
                    && _endTimeSupplied.HasValue
                    && _target.Start != default(LocalTime)
                )
                {
                    _target.Period = NodaTime.Period.Between(_target.Start, _endTimeSupplied.Value);
                }

                _target.Validate();

                return _target;
            }
        }
    }
}