using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NodaTime;

namespace Scheduler
{
    [DataContract]
    public class CompositeSchedule : ISchedule
    {
        public List<ISchedule> Inclusions = new Schedules();
        [DataMember]
        public List<ISchedule> Exclusions = new Schedules();
        [DataMember]
        public List<Range> Breaks = new List<Range>();

        public CompositeSchedule()
        {

        }

        public IEnumerable<LocalDate> Dates
        {
            get
            {
                var inclusions = Inclusions.SelectMany(i => i.Dates);
                var exclusions = Exclusions.SelectMany(e => e.Dates);

                var list = inclusions.Exclude(exclusions);

                list = list.Exclude(Breaks);

                return list;
            }
        }
    }
}
