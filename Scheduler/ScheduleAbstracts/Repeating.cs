using System;
using NodaTime;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NUnit.Framework;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Scheduler.ScheduleAbstracts
{
    public abstract class Repeating : Schedule
    {
        public Range Range;
        public int? CountFrom;
        public int? CountTo;
        public int CountFromDefault { get; set; }
        public int CountToDefault { get; set; }
        protected int Increment = 1;

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            throw new NotImplementedException();
        }
    }
}
