﻿using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler
{
    public interface ICompositeSchedule : IScheduleInstance
    {
        IEdgeVertexs<ISchedule> Inclusions { get; set; }
        IEdgeVertexs<ISchedule> Exclusions { get; set; }
        IEdgeVertexs<IRangeDate> Breaks { get; set; }
    }
}
