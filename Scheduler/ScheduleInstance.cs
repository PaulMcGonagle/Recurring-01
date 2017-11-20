using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public abstract class ScheduleInstance
    {
        public abstract IEnumerable<IDate> Generate(IClock clock);

        public string TypeName => GetType().FullName;

        public virtual void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            
        }
    }
}
