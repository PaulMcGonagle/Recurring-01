using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class ByDateList : Schedule
    {
        [IgnoreDataMember]
        public IEdgeVertexs<IDate> Items
        {
            get;
            set;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            return Items.Select(item => item.ToVertex);
        }

        public static ByDateList Create(
            IEnumerable<IDate> dates)
        {
            return new ByDateList
            {
                Items = new EdgeVertexs<IDate>(dates)
            };
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<ByDateList>(db);

            foreach (var item in Items)
            {
                item.Save(db, clock, this);
            }

            base.Save(db, clock);
        }
    }
}
