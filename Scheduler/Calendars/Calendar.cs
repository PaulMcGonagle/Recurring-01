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
        public IEdgeVertexs<IEvent> Events { get; set; }

        public string Description { get; set; }

        public override void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Description, nameof(Description));
            Guard.AgainstNull(Events, nameof(Events));
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Calendar>(db);
            Events.Save(db, clock, this);
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Events = new EdgeVertexs<IEvent>(Utilities.GetEdges<IEvent>(db, Id));

            base.Rehydrate(db);
        }

        #endregion

        public class Builder : Builder<Calendar>
        {
            public string Description
            {
                set => _target.Description = value;
            }

            public EdgeVertexs<IEvent> Events
            {
                set => _target.Events = value;
            }
        }
    }
}
