using System.Collections.Generic;
using System.Linq;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerial
    {
        public IEnumerable<Episode> Episodes
        {
            get { return this.SelectMany(ce => ce.Episodes); }
        }
    }
}
