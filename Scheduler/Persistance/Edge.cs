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
        public Vertex FromVertex { get; set; }

        [IgnoreDataMember]
        public IVertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId => FromVertex.Id;

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId => ToVertex.Id;

        #region Save

        public void Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            FromVertex = fromVertex;

            if (!FromVertex.IsPersisted)
                throw new SaveException(SaveResult.Incomplete, this.GetType(), $"FromVertex has not been persisted ({FromVertex.ToString()})");

            ToVertex.Save(db, clock);

            Save<Edge>(db);
        }

        #endregion
    }
}
