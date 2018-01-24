using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public interface IScheduleInstance
    {
        IEnumerable<IDate> Generate(IClock clock);

        IEnumerable<IDate> ApplyFilter(IClock clock, IEnumerable<IDate> inputDates);

        string TypeName { get; }

        void Save(IArangoDatabase db, IClock clock, ISchedule schedule);

    }
}
