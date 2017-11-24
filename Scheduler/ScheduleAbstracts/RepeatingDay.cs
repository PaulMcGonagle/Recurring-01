using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class RepeatingDay : Repeating
    {
        public enum RollStrategyType
        {
            Back,
            Skip,
            Forward,
            Throw,
        }

        public RollStrategyType RollStrategy = RollStrategyType.Skip;

        public new abstract class Builder : Repeating.Builder
        {
            protected abstract RepeatingDay RepeatingDay { get; }

            protected override Repeating Repeating => RepeatingDay;

            public EdgeRangeDate EdgeRange
            {
                set => Repeating.EdgeRangeDate = value;
            }

            public IRangeDate Range
            {
                set => Repeating.EdgeRangeDate = new EdgeRangeDate(value);
            }

            public RepeatingDay.RollStrategyType RollStrategy
            {
                set => RepeatingDay.RollStrategy = value;
            }
        }
    }
}