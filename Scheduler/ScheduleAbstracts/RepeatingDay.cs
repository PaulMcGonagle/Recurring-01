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

        [DataMember]
        public RollStrategyType RollStrategy = RollStrategyType.Skip;
    }
}
