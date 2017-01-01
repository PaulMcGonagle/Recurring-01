using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
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

        [IgnoreDataMember]
        public override IEnumerable<Date> Dates
        {
            get
            {
                var inclusions = InclusionsEdges.SelectMany(i => i.ToVertex.Dates);
                var exclusions = ExclusionsEdges.SelectMany(i => i.ToVertex.Dates);

                var list = inclusions.Exclude(exclusions);

                list = list.Exclude(Breaks);

                return list;
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {

            return Save(new Func<SaveResult>[]
            {
                () => Save<CompositeSchedule>(db),
                () => InclusionsEdges.Save(db, this),
                () => ExclusionsEdges.Save(db, this),
                () => base.Save(db),
            });
        }
    }
}
