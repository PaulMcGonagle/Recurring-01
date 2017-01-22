﻿using System;
using System.Collections.Generic;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Scheduler.Generation
{
    public class GeneratedDate : Vertex, IGeneratedDate
    {
        public Date Date { get; set; }

        public EdgeSchedule Source { get; set; }

        public GeneratedDate(Schedule source, Date date)
        {
            Source = new EdgeSchedule(source);

            Date = date;
        }

        int IComparable.CompareTo(object obj)
        {
            return Date.Value.CompareTo(((GeneratedDate)obj).Date.Value);
        }
    }
}
