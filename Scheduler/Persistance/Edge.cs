using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using ArangoDB.Client.Data;

namespace Scheduler.Persistance
{
    public class Edge : Vertex
    {
        [IgnoreDataMember]
        public Vertex FromVertex { get; set; }

        [IgnoreDataMember]
        public Vertex ToVertex { get; set; }

        [DocumentProperty(Identifier = IdentifierType.EdgeFrom)]
        public string FromId => FromVertex.Id;

        [DocumentProperty(Identifier = IdentifierType.EdgeTo)]
        public string ToId => ToVertex.Id;

        #region Save

        public SaveResult Save(IArangoDatabase db, Vertex fromVertex)
        {
            FromVertex = fromVertex;

            if (!FromVertex.IsPersisted)
                return Vertex.SaveResult.Incomplete;

            return Save(new Func<SaveResult>[]
            {
                () => ToVertex.Save(db),
                () => Save<Edge>(db),
            });
        }

        #endregion
    }
}
