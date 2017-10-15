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
        private enum RelationLabels
        {
            Inclusions,
            Exclusions,
            Breaks
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
                .SelectMany(i => i.ToVertex.Generate(clock));

            var exclusions = Exclusions
                .SelectMany(i => i.ToVertex.Generate(clock));

            var list = new List<IDate>();

            list.AddRange(inclusions);
            list.RemoveAll(l => exclusions.Select(e => e.Value).Contains(l.Value));

            foreach (var @break in Breaks)
            {
                list.RemoveAll(d => @break.ToVertex.Contains(d.Value));
            }

            return list;
        }

        public static CompositeSchedule Create(
            ISchedule schedule,
            IRangeDate rangeDate
            )
        {
            return new CompositeSchedule
            {
                Inclusions = new EdgeVertexs<ISchedule>()
                {
                    new EdgeVertex<ISchedule>(new ByWeekday(
                        weekday: IsoDayOfWeek.Wednesday)
                        {
                            EdgeRange = new EdgeRangeDate(rangeDate),
                        }
                    )
                },
            };
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<CompositeSchedule>(db);
            Inclusions.Save(db, clock, this, "Inclusions");
            Exclusions.Save(db, clock, this, "Exclusions");
            Breaks.Save(db, clock, this, "Breaks");
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Inclusions = new EdgeVertexs<ISchedule>();

            Inclusions.AddRange(Utilities.GetByToId<ByDateList>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<ByDayOfMonth>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<ByDayOfYear>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<ByWeekday>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<SingleDay>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<ByOffset>(db, Id, "Inclusions"));
            Inclusions.AddRange(Utilities.GetByToId<ByWeekdays>(db, Id, "Inclusions"));

            Breaks = new EdgeVertexs<IRangeDate>();

            Breaks.AddRange(Utilities.GetByToId<RangeDate>(db, Id, "Inclusions"));

            base.Rehydrate(db);
        }
    }
}
