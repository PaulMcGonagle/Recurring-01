using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using ArangoDB.Client.Data;
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

        public SaveResult Save(IArangoDatabase db, IClock clock, Vertex fromVertex)
        {
            FromVertex = fromVertex;

            if (!FromVertex.IsPersisted)
                return Vertex.SaveResult.Incomplete;

            return Save(new Func<SaveResult>[]
            {
                () => ToVertex.Save(db, clock),
                () => Save<Edge>(db),
            });
        }

        #endregion
    }
}
