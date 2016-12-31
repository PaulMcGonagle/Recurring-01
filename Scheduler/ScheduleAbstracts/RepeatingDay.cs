using System.Runtime.Serialization;

namespace Scheduler.ScheduleAbstracts
{
    [DataContract]
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
}
