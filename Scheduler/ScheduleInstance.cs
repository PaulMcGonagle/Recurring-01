using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public abstract class ScheduleInstance : IScheduleInstance
    {
        public abstract IEnumerable<IDate> Generate(IClock clock);

        public virtual IEnumerable<IDate> Generate(IClock clock, IEnumerable<IDate> source)
        {
            return Generate(clock)
                .Where(source.Contains);
        }

        public string TypeName => GetType().FullName;

        public abstract void Validate();

        public virtual void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            // no bespoke Save functionality required
        }
    }
}
