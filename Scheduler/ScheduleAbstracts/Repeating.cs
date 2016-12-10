using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NUnit.Framework;
using Scheduler.ScheduleInstances;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : PersistableEntity, ISchedule
    {
        public Range Range;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }
        protected int Increment = 1;

        public abstract IEnumerable<LocalDate> Dates { get; }

        public void Save(IArangoDatabase db)
        {
            Save<Repeating>(db);
        }
    }
}
