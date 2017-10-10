using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.ScheduleInstances
{
    public class DateList : Schedule
    {
        public IEnumerable<IDate> Items
        {
            get;
            set;
        }

        public override IEnumerable<IDate> Generate(IClock clock)
        {
            return Items;
        }

        public static DateList Create(
            IEnumerable<IDate> dates)
        {
            return new DateList
            {
                Items = dates.ToList()
            };
        }

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<DateList>(db);

            foreach (var item in Items)
            {
                item.Save(db, clock);
            }

            base.Save(db, clock);
        }
    }
}
