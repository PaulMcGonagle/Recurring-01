using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Scheduler
{
    public class CompositeSchedule : Schedule, ICompositeSchedule
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

        private IEnumerable<ISchedule> GetSchedules(IArangoDatabase db, string relationLabel)
        {
            var schedules = new List<ISchedule>();

            schedules.AddRange(Utilities.GetEdges<Schedule>(db, Id, relationLabel));

            return schedules;
        }

        #region Persistance

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<CompositeSchedule>(db);
            Inclusions.Save(db, clock, this, RelationLabels.Inclusions);
            Exclusions.Save(db, clock, this, RelationLabels.Exclusions);
            Breaks.Save(db, clock, this, RelationLabels.Breaks);
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Inclusions = new EdgeVertexs<ISchedule>();
            Inclusions.AddRange(GetSchedules(db, RelationLabels.Inclusions));
            Exclusions.AddRange(GetSchedules(db, RelationLabels.Exclusions));

            Breaks = new EdgeVertexs<IRangeDate>();

            Breaks.AddRange(Utilities.GetEdges<RangeDate>(db, Id, RelationLabels.Breaks));

            base.Rehydrate(db);
        }

        #endregion
    }

    public class CompositeScheduleBuilder
    {
        private readonly CompositeSchedule _compositeSchedule;

        public CompositeScheduleBuilder()
        {
            _compositeSchedule = new CompositeSchedule();
        }

        public IEdgeVertexs<ISchedule> Inclusions
        {
            set => _compositeSchedule.Inclusions = value;
        }

        public IEdgeVertexs<ISchedule> Exclusions
        {
            set => _compositeSchedule.Exclusions = value;
        }

        public IEdgeVertexs<IRangeDate> Breaks
        {
            set => _compositeSchedule.Breaks = value;
        }

        public CompositeSchedule Build()
        {
            if (_compositeSchedule.Inclusions == null)
                _compositeSchedule.Inclusions = new EdgeVertexs<ISchedule>();


            if (_compositeSchedule.Exclusions == null)
                _compositeSchedule.Exclusions = new EdgeVertexs<ISchedule>();


            if (_compositeSchedule.Breaks == null)
                _compositeSchedule.Breaks = new EdgeVertexs<IRangeDate>();

            return _compositeSchedule;
        }
    }
}
