using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class FilteredSchedule : ScheduleInstance
    {
        private static class RelationLabels
        {
            public const string Filters = "Filters";
        }

        [IgnoreDataMember]
        public IEdgeVertex<ISchedule> Inclusion { get; set; }

        [IgnoreDataMember]
        public IEdgeVertexs<ISchedule> Filters { get; set; } = new EdgeVertexs<ISchedule>();

        [IgnoreDataMember]
        public ISchedule Filter {
            set => Filters = new EdgeVertexs<ISchedule>(value);
        }
        
        public override void Validate()
        {
            Guard.AgainstNull(Inclusion, nameof(Inclusion));
            Guard.AgainstNull(Filters, nameof(Filters));
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            var dates = Inclusion.ToVertex.Generate(clock);

            return Filters.Aggregate(dates, (current, filter) => filter.ToVertex.ScheduleInstance.ApplyFilter(clock, current));
        }

        #region Persistance

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            Inclusion.Save(db, clock, schedule);
            Filters.Save(db, clock, schedule, RelationLabels.Filters);
        }

        #endregion
    }
}
