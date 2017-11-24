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

        public void Validate()
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

        public class Builder
        {
            private readonly RangeTime _rangeTime;
            private LocalTime? _endTimeSupplied;

            public Builder()
            {
                _rangeTime = new RangeTime();
            }

            public LocalTime Start
            {
                set => _rangeTime.Start = value;
            }

            public LocalTime End
            {
                set
                {
                    _endTimeSupplied = value;
                    _rangeTime.Period = default(Period);
                }
            }

            public Period Period
            {
                set
                {
                    _rangeTime.Period = value;
                    _endTimeSupplied = null;
                }
            }

            public RangeTime Build()
            {
                if (_rangeTime.Period == null
                    && _endTimeSupplied.HasValue
                    && _rangeTime.Start != default(LocalTime)
                )
                {
                    _rangeTime.Period = NodaTime.Period.Between(_rangeTime.Start, _endTimeSupplied.Value);
                }

                _rangeTime.Validate();

                return _rangeTime;
            }
        }
    }
}