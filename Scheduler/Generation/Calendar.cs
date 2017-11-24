using System;
using System.Linq;
using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Generation
{
    public class Calendar : Vertex, ICalendar
    {
        [IgnoreDataMember]
        public IEdgeVertexs<IDate> Dates { get; set; }

        public string Description { get; set; }

        public void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Description, nameof(Description));
            Guard.AgainstNull(Dates, nameof(Dates));
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

    public class CalendarBuilder
    {
        private readonly Calendar _calendar;

        public CalendarBuilder()
        {
            _calendar = new Calendar();
        }

        public string Description
        {
            set => _calendar.Description = value;
        }

        public EdgeVertexs<IDate> Dates
        {
            set => _calendar.Dates = value;
        }

        public ICalendar Build()
        {
            return _calendar;
        }
    }
}
