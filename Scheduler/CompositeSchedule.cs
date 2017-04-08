using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Scheduler
{
    public class CompositeSchedule : Schedule
    {

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> InclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> ExclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<IDateRange> Breaks = new EdgeVertexs<IDateRange>();

        public override IEnumerable<IDate> Generate()
        {
            var inclusions = InclusionsEdges.SelectMany(i => i.ToVertex.Generate());
            var exclusions = ExclusionsEdges.SelectMany(i => i.ToVertex.Generate());

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
            IClock clock, 
            Schedule schedule,
            DateRange dateRange
            )
        {
            return new CompositeSchedule
            {
                InclusionsEdges = new EdgeVertexs<ISchedule>()
                {
                    new EdgeVertex<ISchedule>(new ByWeekday(
                        weekday: IsoDayOfWeek.Wednesday,
                        clock: clock)
                        {
                            EdgeRange = new EdgeRangeDate(dateRange),
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
