using System.Collections.Generic;

namespace Scheduler
{
    public interface ISerial
    {
        IEnumerable<Episode> Episodes();
    }
}
