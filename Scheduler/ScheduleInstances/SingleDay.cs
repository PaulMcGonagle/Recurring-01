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

        public class Builder : Builder<SingleDay>
        {
            public Date Date
            {
                set => _target.Date = value;
            }
        }
    }
}