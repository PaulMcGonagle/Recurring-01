﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler.ScheduleInstances
{
    public class SingleDay : Schedule
    {
        public Date Date
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates
        {
            get { yield return Date; }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<SingleDay>(db),
                () => base.Save(db),
            });
        }
    }
}
