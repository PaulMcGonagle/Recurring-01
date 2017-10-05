using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class Relation : Vertex, IRelation
    {
        public string Label { get; set; }

        [IgnoreDataMember]
        public IVertex FromVertex { get; set; }

        [IgnoreDataMember]
        public IVertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId { get; set; }

        #region Save

        public override void Save(IArangoDatabase db, IClock clock)
        {
            if (!FromVertex.IsPersisted)
            {
                FromVertex.Save(db, clock);
            }

            if (!ToVertex.IsPersisted)
            {
                ToVertex.Save(db, clock);
            }

            FromId = FromVertex.Id;
            ToId = ToVertex.Id;

            Save<Relation>(db);
            base.Save(db, clock);
        }

        #endregion

        public override string ToString()
        {
            return $"Relation from {FromVertex} to: {ToVertex}";
        }
    }
}
