using System;
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
using Scheduler.Users;

namespace Scheduler
{
    public class CompositeSchedule : Schedule
    {

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> InclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> ExclusionsEdges { get; set; } = new EdgeVertexs<ISchedule>();

        public IDateRanges Breaks = new DateRanges();

        public override GeneratedDates Generate()
        {
            var inclusions = InclusionsEdges.SelectMany(i => i.ToVertex.Generate());
            var exclusions = ExclusionsEdges.SelectMany(i => i.ToVertex.Generate());

            var list = new GeneratedDates();

            list.AddRange(inclusions);
            list.RemoveAll(l => exclusions.Select(e => e.Date.Value).Contains(l.Date.Value));

            var removeAll = list.RemoveAll(d => Breaks.Contains(d.Date.Value));

            return list;
        }

        public static CompositeSchedule Create(
            IClock clock, 
            Schedule schedule,
            DateRange dateRange
            //string timeZoneProvider, 
            //Location location = null
            )
        {
            return new CompositeSchedule
            {
                InclusionsEdges = new EdgeVertexs<ISchedule>()
                {
                    new EdgeVertex<ISchedule>(new ByWeekday(
                        clock: clock,
                        weekday: IsoDayOfWeek.Wednesday)
                        {
                            EdgeRange = new EdgeRange(dateRange),
                        }
                    )
                },
            };
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {

            return Vertex.Save(new Func<SaveResult>[]
            {
                () => Save<CompositeSchedule>(db),
                () => InclusionsEdges.Save(db, clock, this),
                () => ExclusionsEdges.Save(db, clock, this),
                () => base.Save(db, clock),
            });
        }
    }
}
