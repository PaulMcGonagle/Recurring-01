using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public interface ICalendar : IVertex
    {
        IEnumerable<IDate> Dates { get; set; }
    }
}
