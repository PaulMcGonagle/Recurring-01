using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.ScheduleInstances
{
    public class ByDateList : ScheduleInstance
    {
        [IgnoreDataMember]
        public IEdgeVertexs<IDate> Items { get; set; }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            return Items.Select(item => item.ToVertex);
        }

        public override void Validate()
        {
            Guard.AgainstNull(Items, nameof(Items));
        }

        public override void Save(IArangoDatabase db, IClock clock, ISchedule schedule)
        {
            foreach (var item in Items)
            {
                item.Save(db, clock, schedule);
            }
        }

        public class Builder
        {
            private readonly ByDateList _byDateList;

            public Builder()
            {
                _byDateList = new ByDateList();
            }

            public IEdgeVertexs<IDate> Items
            {
                set => _byDateList.Items = value;
            }

            public ByDateList Build()
            {
                if (_byDateList.Items == null)
                    _byDateList.Items = new EdgeVertexs<IDate>();

                return _byDateList;
            }
        }
    }
}