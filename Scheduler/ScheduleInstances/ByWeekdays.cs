using NodaTime;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary;
using Scheduler.ScheduleAbstracts;

namespace Scheduler.ScheduleInstances
{
    public class ByWeekdays : ScheduleAbstracts.Repeating
    {
        public IEnumerable<IsoDayOfWeek> Weekdays;

        public ByWeekdays(
            IEnumerable<IsoDayOfWeek> weekdays)
        {
            Weekdays = weekdays;
        }

        public ByWeekdays()
        {
            CountFromDefault = 0;
            CountToDefault = 52;
        }

        public override void Validate()
        {
            base.Validate();

            Guard.AgainstNull(Weekdays, nameof(Weekdays));
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var start = EdgeRangeDate.ToVertex.Start.Date ??
                        DateTimeHelper.GetToday(clock).AddWeeks(-(CountFrom ?? CountFromDefault));
            var end = EdgeRangeDate.ToVertex.End.Date ?? DateTimeHelper.GetToday(clock)
                          .AddWeeks((CountTo ?? CountToDefault));

            var range = DateTimeHelper.Range(start: start, end: end);

            return range.Where(d => Weekdays.Contains(d.IsoDayOfWeek));
        }

        public new class Builder : Repeating.Builder
        {
            private readonly ByWeekdays _byWeekdays;

            protected override Repeating Repeating => _byWeekdays;

            public Builder()
            {
                _byWeekdays = new ByWeekdays();
            }

            public IEnumerable<IsoDayOfWeek> Weekdays
            {
                get => _byWeekdays.Weekdays;
                set => _byWeekdays.Weekdays = value;
            }

            public ByWeekdays Build()
            {
                _byWeekdays.Validate();

                return _byWeekdays;
            }
        }
    }
}