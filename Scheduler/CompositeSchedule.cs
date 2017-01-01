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

        public CompositeSchedule()
        {

        }

        [IgnoreDataMember]
        public override IEnumerable<Scheduler.Date> Dates
        {
            get
            {
                var inclusions = InclusionsEdges.SelectMany(i => ((Schedule)i.ToVertex).Dates);
                var exclusions = ExclusionsEdges.SelectMany(i => ((Schedule)i.ToVertex).Dates);

                var list = inclusions.Exclude(exclusions);

                list = list.Exclude(Breaks);

                return list;
            }
        }

        public override SaveResult Save(IArangoDatabase db)
        {
            var result = Save<CompositeSchedule>(db);

            if (result != SaveResult.Success)
                return result;

            result = InclusionsEdges.Save(db, this);
            
            if (result != SaveResult.Success)
                return result;

            ExclusionsEdges.Save(db, this);


            if (result != SaveResult.Success)
                return result;

            return SaveResult.Success;
        }
    }
}
