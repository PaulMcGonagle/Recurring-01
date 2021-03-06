﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreLibrary;
using NodaTime;
using Scheduler.Ranges;
using Scheduler.ScheduleAbstracts;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleInstances
{
    public class ByOffset : Repeating
    {
        [DataMember] public LocalDate InitialDate;
        [DataMember] public string Interval;

        public override void Validate()
        {
            Guard.AgainstNull(InitialDate, nameof(InitialDate));
            Guard.AgainstNullOrWhiteSpace(Interval, nameof(Interval));
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var results = new List<IDate>();

            EdgeRangeDate?.RangeDate.Validate();

            var iterDate = InitialDate;

            while (EdgeRangeDate?.RangeDate.Contains(iterDate) == true
                   && results.Count <= (CountTo ?? CountToDefault))
            {
                results.Add(new Date(iterDate));

                iterDate = DateAdjuster.Adjust(iterDate, Interval);
            }

            results.Sort();

            return results;
        }

        public new class Builder : Repeating.Builder
        {
            private readonly ByOffset _byOffset;

            protected override Repeating Repeating => _byOffset;

            public Builder()
            {
                _byOffset = new ByOffset();
            }

            public LocalDate InitialDate
            {
                set => _byOffset.InitialDate = value;
            }

            public string Interval
            {
                set => _byOffset.Interval = value;
            }

            public IRangeDate Range
            {
                set => _byOffset.EdgeRangeDate = new EdgeRangeDate(value);
            }

            public ByOffset Build()
            {
                _byOffset.Validate();

                return _byOffset;
            }
        }
    }
}