using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Scheduler.ScheduleInstances
{
    [DataContract]
    public class SingleDay : ISchedule
    {
        public SingleDay()
        {
        }

        [DataMember]
        public LocalDate Date
        {
            get;
            set;
        }

        public IEnumerable<LocalDate> Dates
        {
            get { yield return Date; }
        }        
    }
}
