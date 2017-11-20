using System;
using System.Collections.Generic;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : ScheduleInstance
    {
        public Date Date
        {
            get;
            set;
        }

        public void Validate()
        {
            Guard.AgainstNull(Date, nameof(Date));
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            yield return Date;
        }
    }

    public class SingleDayBuilder
    {
        private readonly SingleDay _singleDay;

        public SingleDayBuilder()
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
