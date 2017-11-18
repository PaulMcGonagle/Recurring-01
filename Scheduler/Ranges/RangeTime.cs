using System;
using System.Globalization;
using System.Runtime.Serialization;
using ArangoDB.Client;
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

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<RangeTime>(db);
            base.Save(db, clock);
        }
    }

    public class RangeTimeBuilder
    {
        private RangeTime _rangeTime;
        private LocalTime? _endTimeSupplied;

        public RangeTimeBuilder()
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
                Period = default(Period);
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
            if (_rangeTime.Start == default(LocalTime))
            {
                throw new ArgumentNullException("Start");
            }

            if (_rangeTime.Period.Equals(default(Period))
            && _endTimeSupplied == null)
            {
                throw new ArgumentNullException("Period");
            }

            if (_rangeTime.Start > _rangeTime.End)
                throw new ArgumentOutOfRangeException(nameof(_rangeTime.Start), $"Start time [{_rangeTime.End.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than To date [{_rangeTime.Start.ToString("D", CultureInfo.CurrentCulture)}]");

            _rangeTime.Period = Period.Between(_rangeTime.Start, _rangeTime.End);

            return _rangeTime;
        }
    }
}
