using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler.Persistance
{
    public class Edge : Vertex
    {
        [IgnoreDataMember]
        public IVertex FromVertex { get; set; }

        [IgnoreDataMember]
        public IVertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId { get; set; }

        #region Save

        public void Save(IArangoDatabase db, IClock clock, IVertex fromVertex)
        {
            FromVertex = fromVertex;

            FromId = fromVertex.Id;

            if (!FromVertex.IsPersisted)
                throw new SaveException(SaveResult.Incomplete, this.GetType(), $"FromVertex has not been persisted ({FromVertex.ToString()})");

            ToVertex.Save(db, clock);

            ToId = ToVertex.Id;

            Save<Edge>(db);
        }

        #endregion
    }
}
