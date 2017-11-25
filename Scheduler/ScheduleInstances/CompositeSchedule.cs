using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Scheduler.ScheduleInstances
{
    public class CompositeSchedule : ScheduleInstance, ICompositeSchedule
    {
        private static class RelationLabels
        {
            public const string Inclusions = "Inclusions";
            public const string Exclusions = "Exclusions";
            public const string Breaks = "Breaks";
        }


        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> Inclusions { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> Exclusions { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<IRangeDate> Breaks { get; set; } = new EdgeVertexs<IRangeDate>();

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var inclusions = Inclusions
                .SelectMany(i => i.ToVertex.Generate(clock))
                .ToList();

            var exclusions = Exclusions
                .SelectMany(i => i.ToVertex.Generate(clock))
                .ToList();

            var list = new List<IDate>();

            list.AddRange(inclusions);
            list.RemoveAll(l => exclusions.Select(e => e.Value).Contains(l.Value));

            foreach (var @break in Breaks)
            {
                list.RemoveAll(d => @break.ToVertex.Contains(d.Value));
            }

            return list;
        }

        public override void Validate()
        {
            Guard.AgainstNull(Inclusions, nameof(Inclusions));
            Guard.AgainstNull(Exclusions, nameof(Exclusions));
            Guard.AgainstNull(Breaks, nameof(Breaks));

            if (Inclusions.Count == 0)
            {
                throw new Exception("CompositeSchedule must have at least one Inclusion schedule");
            }
        }

        private IEnumerable<ISchedule> GetSchedules(IArangoDatabase db, string relationLabel, ISchedule schedule)
        {
            var schedules = new List<ISchedule>();

            schedules.AddRange(Utilities.GetEdges<Schedule>(db, schedule.Id, relationLabel));

            return schedules;
        }

        #region Persistance

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            Inclusions.Save(db, clock, schedule, RelationLabels.Inclusions);
            Exclusions.Save(db, clock, schedule, RelationLabels.Exclusions);
            Breaks.Save(db, clock, schedule, RelationLabels.Breaks);
        }

        public void Rehydrate(IArangoDatabase db, ISchedule schedule)
        {
            Inclusions = new EdgeVertexs<ISchedule>();
            Inclusions.AddRange(GetSchedules(db, RelationLabels.Inclusions, schedule));
            Exclusions.AddRange(GetSchedules(db, RelationLabels.Exclusions, schedule));

            Breaks = new EdgeVertexs<IRangeDate>();

            Breaks.AddRange(Utilities.GetEdges<RangeDate>(db, schedule.Id, RelationLabels.Breaks));
        }

        #endregion

        public class Builder : ScheduleInstance.Builder<CompositeSchedule>
        {
            public IEdgeVertexs<ISchedule> Inclusions
            {
                set => _target.Inclusions = value;
            }

            public IEdgeVertexs<ISchedule> Exclusions
            {
                set => _target.Exclusions = value;
            }

            public IEdgeVertexs<IRangeDate> Breaks
            {
                set => _target.Breaks = value;
            }
        }
    }
}