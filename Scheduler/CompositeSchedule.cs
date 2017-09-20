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

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> InclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> ExclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<IRangeDate> Breaks { get; set; } = new EdgeVertexs<IRangeDate>();

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var inclusions = InclusionsEdges.SelectMany(i => i.ToVertex.Generate(clock));
            var exclusions = ExclusionsEdges.SelectMany(i => i.ToVertex.Generate(clock));

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
                InclusionsEdges = new EdgeVertexs<ISchedule>()
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
            InclusionsEdges.Save(db, clock, this);
            ExclusionsEdges.Save(db, clock, this);
            Breaks.Save(db, clock, this);
            base.Save(db, clock);
        }
    }
}
