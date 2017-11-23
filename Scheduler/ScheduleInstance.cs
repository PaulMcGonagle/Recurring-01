using System.Collections.Generic;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public abstract class ScheduleInstance : IScheduleInstance
    {
        public abstract IEnumerable<IDate> Generate(IClock clock);

        public string TypeName => GetType().FullName;

        public abstract void Validate();

        public virtual void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            
        }
    }
}
