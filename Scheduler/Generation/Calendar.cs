using System;
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
        public IEdgeVertexs<IDate> Dates { get; set; }

        public static Calendar Create(IClock clock, ISchedule source)
        {
            if (source.IsDirty)
                throw new ArgumentException("Event has not yet been persisted");

            var calendar = new Calendar
            {
                Dates = new EdgeVertexs<IDate>(
                    source
                        .Generate(clock)
                        .ToList())
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
            Dates.Save(db, clock, this);
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Dates = new EdgeVertexs<IDate>(Utilities.GetEdges<Date>(db, Id));

            base.Rehydrate(db);
        }

        #endregion
    }
}
