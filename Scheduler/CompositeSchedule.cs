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
        public Edges InclusionsEdges = new Edges();
        [IgnoreDataMember]
        public Edges ExclusionsEdges = new Edges();
        //public List<ISchedule> Inclusions = new Schedules();
        //public List<ISchedule> Exclusions = new Schedules();
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

            InclusionsEdges.Save(db, this);

            return SaveResult.Success;
        }
    }
}
