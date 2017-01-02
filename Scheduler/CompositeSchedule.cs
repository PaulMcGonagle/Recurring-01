using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class CompositeSchedule : Schedule
    {

        [IgnoreDataMember]
        public EdgeVertexs<Schedule> InclusionsEdges { get; set; } = new EdgeVertexs<Schedule>();

        [IgnoreDataMember]
        public EdgeVertexs<Schedule> ExclusionsEdges { get; set; } = new EdgeVertexs<Schedule>();

        public List<Range> Breaks = new List<Range>();

        public override IEnumerable<Date> GenerateDates()
        {
            var inclusions = InclusionsEdges.SelectMany(i => i.ToVertex.GenerateDates());
            var exclusions = ExclusionsEdges.SelectMany(i => i.ToVertex.GenerateDates());

            var list = inclusions.Exclude(exclusions);

            list = list.Exclude(Breaks);

            return list;
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {

            return Save(new Func<SaveResult>[]
            {
                () => Save<CompositeSchedule>(db),
                () => InclusionsEdges.Save(db, clock, this),
                () => ExclusionsEdges.Save(db, clock, this),
                () => base.Save(db, clock),
            });
        }
    }
}
