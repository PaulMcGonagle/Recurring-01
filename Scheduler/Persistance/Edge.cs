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

        public SaveResult Save(IArangoDatabase db, Vertex fromVertex)
        {
            FromVertex = fromVertex;

            ToVertex.Save(db);

            return Save<Edge>(db);
        }
    }
}
