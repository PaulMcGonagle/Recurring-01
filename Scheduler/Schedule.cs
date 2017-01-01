﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public abstract class Schedule : Vertex, ISchedule
    {
        [IgnoreDataMember]
        public virtual IEnumerable<Scheduler.Date> Dates { get; }
    }
}
