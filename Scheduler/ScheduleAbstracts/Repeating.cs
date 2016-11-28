using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Scheduler.ScheduleAbstracts
{
    [DataContract]
    public abstract class Repeating : ISchedule
    {
        [DataMember]
        public Range Range;
        [DataMember]
        public int? CountFrom;
        [DataMember]
        public int? CountTo;
        [DataMember]
        public int CountFromDefault { get; set; }
        [DataMember]
        public int CountToDefault { get; set; }
        [DataMember]
        protected int Increment = 1;

        public abstract IEnumerable<LocalDate> Dates { get; }
    }
}
