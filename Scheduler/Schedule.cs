using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Schedule : Vertex, ISchedule
    {
        protected Schedule()
        {
            
        }

        [IgnoreDataMember]
        public virtual IEnumerable<Scheduler.Date> Dates { get; }
    }
}
