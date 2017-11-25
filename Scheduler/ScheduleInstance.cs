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

        public abstract class Builder<T> where T : ScheduleInstance, new()
        {
            protected readonly T _target;

            protected Builder()
            {
                _target = new T();
            }

            public T Build()
            {
                _target.Validate();

                return _target;
            }
        }
    }
}
