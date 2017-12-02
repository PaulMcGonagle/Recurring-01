using System.Runtime.Serialization;
using ArangoDB.Client;
using CoreLibrary;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler.Calendars
{
    public class Calendar : Vertex, ICalendar
    {
        [IgnoreDataMember]
        public IEdgeVertexs<IDate> Dates { get; set; }

        public string Description { get; set; }

        public override void Validate()
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

        public class Builder : Builder<Calendar>
        {
            public string Description
            {
                set => _target.Description = value;
            }

            public EdgeVertexs<IDate> Dates
            {
                set => _target.Dates = value;
            }
        }
    }
}
