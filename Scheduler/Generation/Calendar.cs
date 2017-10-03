using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public class Calendar : Vertex, ICalendar
    {
        [IgnoreDataMember]
        private IRelation Source { get; set; }

        public IEnumerable<IDate> Dates { get; set; }

        public static Calendar Generate(IClock clock, ISchedule source)
        {
            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            var calendar = new Calendar
            {
                Dates = source
                    .Generate(clock)
                    .ToList()
            };

            calendar
                .Tags
                .AddRange(source.Tags);

            return calendar;
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Calendar>(db);
            base.Save(db, clock);
        }

        #endregion
    }
}
