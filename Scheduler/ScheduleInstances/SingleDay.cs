using System.Collections.Generic;
using CoreLibrary;
using NodaTime;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : ScheduleInstance
    {
        public Date Date { get; set; }

        public override void Validate()
        {
            Guard.AgainstNull(Date, nameof(Date));
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            yield return Date;
        }

        public class Builder
        {
            private readonly SingleDay _singleDay;

            public Builder()
            {
                _singleDay = new SingleDay();
            }

            public Date Date
            {
                set => _singleDay.Date = value;
            }

            public SingleDay Build()
            {
                _singleDay.Validate();

                return _singleDay;
            }
        }
    }
}