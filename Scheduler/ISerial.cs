using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler
{
    public interface ISerial : IVertex
    {
        IEnumerable<Episode> Episodes { get; }
    }
}
