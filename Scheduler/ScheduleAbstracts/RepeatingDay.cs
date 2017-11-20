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
    }

    public abstract class RepeatingDayBuilder : RepeatingBuilder
    {
        protected abstract RepeatingDay RepeatingDay { get; }

        protected override Repeating Repeating => RepeatingDay;

        public EdgeRangeDate EdgeRange
        {
            set => Repeating.EdgeRangeDate = value;
        }

        public int? CountFrom { set => Repeating.CountFrom = value; }
        public int? CountTo { set => Repeating.CountTo = value; }
        public int CountFromDefault { set => Repeating.CountFromDefault = value; }
        public int CountToDefault { set => Repeating.CountToDefault = value; }
    }
}
