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
        public IEdgeVertexs<IEpisode> Episodes { get; set; }

        public string Description { get; set; }

        public override void Validate()
        {
            Guard.AgainstNullOrWhiteSpace(Description, nameof(Description));
            Guard.AgainstNull(Episodes, nameof(Episodes));
        }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            Save<Calendar>(db);
            Episodes.Save(db, clock, this);
            base.Save(db, clock);
        }

        public override void Rehydrate(IArangoDatabase db)
        {
            Episodes = new EdgeVertexs<IEpisode>(Utilities.GetEdges<IEpisode>(db, Id));

            base.Rehydrate(db);
        }

        #endregion

        public class Builder : Builder<Calendar>
        {
            public string Description
            {
                set => _target.Description = value;
            }

            public EdgeVertexs<IEpisode> Episodes
            {
                set => _target.Episodes = value;
            }
        }
    }
}
